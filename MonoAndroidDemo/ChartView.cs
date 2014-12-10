using System;

using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace MonoAndroidDemo
{
  [Activity(Label = "My Chart")]
  public class ChartView : Activity
  {
    Steema.TeeChart.TChart chart;

    protected override void OnCreate(Bundle bundle)
    {
      base.OnCreate(bundle);

      chart = new Steema.TeeChart.TChart (ApplicationContext);

      chart.Zoom.Style = Steema.TeeChart.ZoomStyles.Classic;
      var myTheme = new Steema.TeeChart.Themes.BlackIsBackTheme(chart.Chart);
      myTheme.Apply();

      Bundle extras = Intent.Extras;
      int seriesType = extras.GetInt("SeriesPosition");

      var tmp = Steema.TeeChart.Utils.SeriesTypesOf [seriesType];
      Steema.TeeChart.Styles.Series series;

      this.Title = tmp.ToString().Replace("Steema.TeeChart.Styles.", "");

      //Some series can not work without a parent chart due to internal structure.
      if (tmp.Name == "TreeMap")
      {
        series = new Steema.TeeChart.Styles.TreeMap(chart.Chart);
      }
      else if (tmp.Name == "PolarGrid")
      {
        series = new Steema.TeeChart.Styles.PolarGrid(chart.Chart);
      }
      else
      {
        series = chart.Series.Add(tmp); 
      }

      series.FillSampleValues();

      chart.Aspect.View3D = Needs3D(chart[0]);
      //chart.Panel.Transparent = true;
      if (chart[0] is Steema.TeeChart.Styles.Pie)
      {
        var pie = (Steema.TeeChart.Styles.Pie)chart[0];
        pie.EdgeStyle = Steema.TeeChart.Drawing.EdgeStyles.Flat;
        pie.BevelPercent = 30;

        chart.Legend.Visible = false;
        chart.Aspect.Elevation = 300;
      }

      if (chart[0] is Steema.TeeChart.Styles.Gantt || chart[0] is Steema.TeeChart.Styles.Funnel)
      {
        chart.Legend.Alignment = Steema.TeeChart.LegendAlignments.Bottom;
      }

      SetContentView(chart);
    }

    bool Needs3D(Steema.TeeChart.Styles.Series series)
    {
      return (((series is Steema.TeeChart.Styles.Custom3D) && !(/*(series is Steema.TeeChart.Styles.ColorGrid)   
                                                            || */(series is Steema.TeeChart.Styles.Contour)
                                                            || (series is Steema.TeeChart.Styles.Map)
                                                            || (series is Steema.TeeChart.Styles.Ternary))) 
                                                            || (series is Steema.TeeChart.Styles.Pie));
    }

    public override bool OnCreateOptionsMenu(IMenu menu)
    {
      menu.Add(Menu.None, 0, Menu.None, "Settings").SetIcon(Android.Resource.Drawable.IcMenuManage);
      menu.Add(Menu.None, 1, Menu.None, "Theme").SetIcon(Android.Resource.Drawable.IcMenuGallery);
      menu.Add(Menu.None, 2, Menu.None, "Share").SetIcon(Android.Resource.Drawable.IcMenuShare);

      return true;// base.OnCreateOptionsMenu(menu);
    }

    public override bool OnOptionsItemSelected(IMenuItem item)
    {
      switch (item.ItemId)
      {
        case 0:
            ((TChartApplication)Application).Chart = chart;
            var editorIntent = new Intent (ApplicationContext, typeof(ChartEditor));
            StartActivityForResult(editorIntent, 1);
            return true;
        case 1:
            var themes = new ThemesEditor(chart.Chart, 0);
            themes.Choose(this);
            return true;
        case 2:
            Java.IO.File cache = ExternalCacheDir;
            if ((cache == null) || (!cache.CanWrite()))
            {
                // no external cache
                cache = CacheDir;
            }

            Android.Graphics.Bitmap _currentBitmap = chart.Bitmap;

            var tempFile = new Java.IO.File(cache, "temp.jpg");
            using (FileStream fileStream = File.OpenWrite(tempFile.AbsolutePath))
            {
              _currentBitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 85, fileStream);
            }

            var shareIntent = new Intent(Intent.ActionSend);
            shareIntent.PutExtra(Intent.ExtraStream, Android.Net.Uri.FromFile(tempFile));
            shareIntent.PutExtra(Intent.ExtraText, "Chart created with TeeChart Mono for Android by www.steema.com");//"Some text - appears in tweets, not on facebook");
            shareIntent.SetType("image/jpeg");

            StartActivity(Intent.CreateChooser(shareIntent, "Share Image"));

            return true;
        default:
            return base.OnOptionsItemSelected(item);
      }      
    }

    
    void DoExport()
    {
      ////TO-DO
      //Intent sendIntent = new Intent(Intent.ActionSend);

      ////Export implementation missing
      //var image = chart.getExport().getImage()
      //    .image(chart.Width, chart.Height);

      //var file = new Java.IO.File("/sdcard/teechart.png");
      //var stream = new Java.IO.FileOutputStream(file);



      //image.Save(stream);
      //stream.Flush();
      //stream.Close();

      //sendIntent.Type = "image/png";
      //sendIntent.PutExtra(Intent.ExtraStream, Uri.parse("file:///sdcard/teechart.png"));

      //StartActivity(Intent.CreateChooser(sendIntent, "Export"));
    }

    void toast(string text)
    {
      Toast
          .MakeText(this, text, ToastLength.Short)
          .Show();
    }
  }
}