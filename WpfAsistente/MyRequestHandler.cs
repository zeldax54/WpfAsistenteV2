using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WpfAsistente
{
    public class MyRequestHandler : CefSharp.Handler.RequestHandler
    {		

		protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser,
			IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
		{
			string x = request.Url;

			return false;
		}



		protected override IResourceRequestHandler GetResourceRequestHandler(
			IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
			IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
			{
				//Where possible only intercept specific Url's
				//Load http://httpbin.org/post in the browser and you'll
				//see the post data
				if (request.Url == "http://httpbin.org/post")
				{
					return new CustomResourceRequestHandler();
				}

				//Default behaviour, url will be loaded normally.
				return null;
			}
		





	}


	public class CustomResourceRequestHandler : CefSharp.Handler.ResourceRequestHandler
	{
		protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser,
			IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
		{
			//Set the header by name, override the existing value
			request.SetHeaderByName("user-agent", "MyBrowser CefSharp Browser", true);

			return CefReturnValue.Continue;
		}
	}
}
