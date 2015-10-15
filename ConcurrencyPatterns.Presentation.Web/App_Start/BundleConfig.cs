using System.Web;
using System.Web.Optimization;

namespace ConcurrencyPatterns.Presentation.Web
{
	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-{version}.js",
						"~/Scripts/jquery-ui-{version}.js"));
			bundles.Add(new StyleBundle("~/Content/css").Include(
					  //"~/Content/bootstrap.css", Add when needed
					  "~/Content/site.css"));
		}
	}
}