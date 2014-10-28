    public class USPS
    { 
        //Base URL for USPS Address and Zip Code validation API. 
        private const string baseURL = "http://production.shippingapis.com/ShippingAPI.dll";
        //Web client instance.
        private WebClient wsClient = new WebClient();
        //User ID obtained from USPS.
        public string USPS_UserID = ConfigurationManager.AppSettings["USPS_USERID"];
	//Maximum package per request
	private const int batchLimit = 25;

	/**
         * Sends Request and gets reply from USPS
         **/
        private string GetResponseFromUSPS(string USPS_Request)
        {
            string strResponse = "";
            //Send the request to USPS.
            byte[] ResponseData = wsClient.DownloadData(USPS_Request);
            //Convert byte stream to string data.
            foreach (byte oItem in ResponseData)
                strResponse += (char)oItem;
            return strResponse;
        } 
	
	/**
	 * Generates request string for getting Rate From USPS 
	 */ 
	public string  GenerateRatev4Request(List<myClass> packages)
	{ 
		string response = "";	
		string request = baseURL + string.Format("?API=RateV4&XML=<RateV4Request USERID=\"{0}\">", USPS_UserID);
		request += "<Revision />"; //I don't use any special services so I don't use Revision element
		
		int counter = 1;  //Define a counter 
		foreach (var package in packages)
		{  	//loop trough each package element that we have and add them into request string
			
			request += string.Format("<Package ID=\"{0}\">", package.ID); //must be unique
			request += string.Format("<Service>{0}</Service>", package.Service);
			request += string.Format("<ZipOrigination>{0}</ZipOrigination>", package.FromOrigin);
			request += string.Format("<ZipDestination>{0}</ZipDestination>",  package.FromDest);
			request += string.Format("<Pounds>{0}</Pounds>", package.PackageWeightLB);
			request += string.Format("<Ounces>{0}</Ounces>", package.PackageWeightOnz);
			request += string.Format("<Container>{0}</Container>",  package.Container);
			request += string.Format("<Size>{0}</Size>",  package.Size);
			request += string.Format("<Width>{0}</Width>", package.Width);
			request += string.Format("<Length>{0}</Length>", package.Length);
			request += string.Format("<Height>{0}</Height>", package.Height);
			request += string.Format("<Girth>{0}</Girth>", package.Girth);
			request += string.Format("</Package>"); 
			
			if (counter % batchLimit == 0)
			{//if number of packages exceed the USPS limit  
				request += string.Format("</RateV4Request>"); //put an end to xml request 
			    
				 //Send the request to USPS and get response
				 response = GetResponseFromUSPS(request);  
				 HandleResponseAsYouWish(response); //Handle the response as you want with another method
				 //Create a new request for the rest of packages
				request = baseURL + string.Format("?API=RateV4&XML=<RateV4Request USERID=\"{0}\" >", USPS_UserID);
				request += "<Revision />"; 
			} 
			counter++;
		}
		//put an end to xml request
		request += string.Format("</RateV4Request>");
	 response = GetResponseFromUSPS(request);  //Send the request to USPS and get response
	 HandleResponseAsYouWish(response); //Handle the response as you want with another method
	}
	
	private void HandleResponseAsYouWish(string response)
        {
			    //Do what you want to do with the response
            throw new NotImplementedException();
        }

}
