using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;

public class CulturesHelper
{
	private static bool _isFoundInstalledCultures = false;

	private static List<CultureInfo> _supportedCultures = new List<CultureInfo>();

	private static ObjectDataProvider _objectDataProvider;

	private static CultureInfo _designTimeCulture = new CultureInfo("zh-TW");

	public static List<CultureInfo> SupportedCultures => _supportedCultures;

	public static ObjectDataProvider ResourceProvider
	{
		get
		{
			if (_objectDataProvider == null)
			{
				_objectDataProvider = (ObjectDataProvider)App.Current.FindResource("Resources");
			}
			return _objectDataProvider;
		}
	}

	public CulturesHelper()
	{
		if (!_isFoundInstalledCultures)
		{
			CultureInfo cultureInfo2 = new CultureInfo("");
			string[] directories = Directory.GetDirectories(System.Windows.Forms.Application.StartupPath);
			foreach (string dir in directories)
			{
				try
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(dir);
					cultureInfo2 = CultureInfo.GetCultureInfo(directoryInfo.Name);
					if (directoryInfo.GetFiles(Path.GetFileNameWithoutExtension(System.Windows.Forms.Application.ExecutablePath) + ".resources.dll").Length != 0)
					{
						_supportedCultures.Add(cultureInfo2);
					}
				}
				catch (ArgumentException)
				{
				}
			}
			if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
			{
				Resources.Culture = _designTimeCulture;
				Settings.Default.DefaultCulture = _designTimeCulture;
			}
			else if (_supportedCultures.Count > 0 && Settings.Default.DefaultCulture != null)
			{
				Resources.Culture = Settings.Default.DefaultCulture;
			}
			_isFoundInstalledCultures = true;
		}
	}

	public Resources GetResourceInstance()
	{
		return new Resources();
	}

	public Resources GetResourceInstance(string cultureName)
	{
		ChangeCulture(new CultureInfo(cultureName));
		return new Resources();
	}

	public static void ChangeCulture(CultureInfo culture)
	{
		if (_supportedCultures.Contains(culture))
		{
			Resources.Culture = culture;
			Settings.Default.DefaultCulture = culture;
			Settings.Default.Save();
			ResourceProvider.Refresh();
		}
	}
}