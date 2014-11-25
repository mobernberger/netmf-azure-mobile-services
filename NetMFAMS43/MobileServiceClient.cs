using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using Json.NETMF;

namespace NetMFAMS43
{
    /// <summary>
    /// Client for Windows Azure Mobile Services via REST API
    /// </summary>
    /// 
    public class MobileServiceClient : IMobileServiceClient
    {
        #region Fields

        // application URI
        private readonly Uri _mobileServicesUri;
        // application key, master key
        private readonly string _applicationKey;
        private readonly string _masterKey;

        private string _finalUri;
        private const string JsonHeader = "application/json";

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mobileServicesUri">Mobile Services URI</param>
        /// <param name="applicationKey">Application Key</param>
        /// <param name="masterKey">Master Key</param>
        /// 
        public MobileServiceClient(Uri mobileServicesUri, string applicationKey = null, string masterKey = null)
        {
            if (mobileServicesUri == null)
            {
                throw new ArgumentNullException("Mobile Service URL parameter cannot be null");
            }

            _mobileServicesUri = mobileServicesUri;
            _applicationKey = applicationKey;
            _masterKey = masterKey;
        }

        /// <summary>
        /// Insert an entity into table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="entity">Entity object</param>
        /// <param name="noscript">NoScript flag </param>
        /// <returns>The Id string which was returned by the mobile service</returns>
        public string Insert(string tableName, IMobileServiceEntityData entity, bool noscript = false)
        {
            // build URI
            _finalUri = _mobileServicesUri.AbsoluteUri + "tables/" + tableName;
            if (noscript)
            {
                if (_masterKey == null)
                    throw new ArgumentException("For noscript you must also supply the Master Key");
                _finalUri = _finalUri + "?noscript=true";
            }

            using (var request = (HttpWebRequest)WebRequest.Create(_finalUri))
            {
                //set-up headers
                var headers = new WebHeaderCollection();
                if (_applicationKey != null)
                {
                    headers.Add("X-ZUMO-APPLICATION", _applicationKey);
                }
                if (_masterKey != null)
                {
                    headers.Add("X-ZUMO-MASTER", _masterKey);
                }
                request.Method = "POST";
                request.Headers = headers;
                request.Accept = JsonHeader;

                if (entity != null)
                {
                    //serialize the data to upload
                    string serialization = JsonSerializer.SerializeObject(entity);

                    //prepare request
                    byte[] byteData = Encoding.UTF8.GetBytes(serialization);
                    request.ContentLength = byteData.Length;
                    request.ContentType = JsonHeader;
                    request.UserAgent = "Micro Framework";

                    using (Stream postStream = request.GetRequestStream())
                    {
                        postStream.Write(
                            byteData,
                            0,
                            byteData.Length
                            );
                    }
                }

                //wait for the response
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    //Check if you are authorized correctly
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return "Please check your Application Key";
                    }

                    //Check if the item was successfully created
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        //deserialize the received data and return the Id
                        var hashtable = JsonSerializer.DeserializeString(reader.ReadToEnd()) as Hashtable;
                        if (hashtable != null)
                        {
                            if (hashtable.Contains("id"))
                            {
                                return hashtable["id"].ToString();
                            }
                        }
                    }

                    //Else return the StatusCode and Description
                    return response.StatusCode + " " + response.StatusDescription;
                }
            }
        }

        /// <summary>
        /// Delete an entity from the table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="entityId">Entity Id</param>
        /// <param name="noscript">NoScript flag </param>
        /// <returns>A string with the result of the operation</returns>
        public HttpStatusCode Delete(string tableName, string entityId, bool noscript = false)
        {
            // build URI
            _finalUri = _mobileServicesUri.AbsoluteUri + "tables/" + tableName + "/" + entityId;
            if (noscript)
            {
                if (_masterKey == null)
                    throw new ArgumentException("For noscript you must also supply the Master Key");
                _finalUri = _finalUri + "?noscript=true";
            }

            using (var request = (HttpWebRequest)WebRequest.Create(_finalUri))
            {
                //set-up headers
                var headers = new WebHeaderCollection();
                if (_applicationKey != null)
                {
                    headers.Add("X-ZUMO-APPLICATION", _applicationKey);
                }
                if (_masterKey != null)
                {
                    headers.Add("X-ZUMO-MASTER", _masterKey);
                }
                request.Method = "DELETE";
                request.Headers = headers;
                request.Accept = JsonHeader;

                request.ContentLength = 0;
                request.ContentType = JsonHeader;
                request.UserAgent = "Micro Framework";

                //wait for the response
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    //Else return the StatusCode and Description
                    return response.StatusCode;
                }
            }
        }

        /// <summary>
        /// Update an entity in the table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="entity">Entity Object</param>
        /// <param name="noscript">NoScript flag </param>
        /// <returns>A string with the result of the operation</returns>
        public HttpStatusCode Update(string tableName, IMobileServiceEntityData entity, bool noscript = false)
        {
            // build URI
            _finalUri = _mobileServicesUri.AbsoluteUri + "tables/" + tableName + "/" + entity.Id;
            if (noscript)
            {
                if (_masterKey == null)
                    throw new ArgumentException("For noscript you must also supply the Master Key");
                _finalUri = _finalUri + "?noscript=true";
            }

            using (var request = (HttpWebRequest)WebRequest.Create(_finalUri))
            {
                //set-up headers
                var headers = new WebHeaderCollection();
                if (_applicationKey != null)
                {
                    headers.Add("X-ZUMO-APPLICATION", _applicationKey);
                }
                if (_masterKey != null)
                {
                    headers.Add("X-ZUMO-MASTER", _masterKey);
                }
                request.Method = "PATCH";
                request.Headers = headers;
                request.Accept = JsonHeader;

                //serialize the data to upload
                string serialization = JsonSerializer.SerializeObject(entity);

                //prepare request
                byte[] byteData = Encoding.UTF8.GetBytes(serialization);
                request.ContentLength = byteData.Length;
                request.ContentType = JsonHeader;
                request.UserAgent = "Micro Framework";

                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(
                        byteData,
                        0,
                        byteData.Length
                        );
                }


                //wait for the response
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    //Else return the StatusCode and Description
                    return response.StatusCode;
                }
            }
        }

        /// <summary>
        /// Query the table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="query">Query</param>
        /// <param name="noscript">NoScript flag </param>
        /// <returns>A string with the result of the operation</returns>
        public string Query(string tableName, string query = null, bool noscript = false)
        {
            // build URI
            _finalUri = _mobileServicesUri.AbsoluteUri + "tables/" + tableName;

            if ((query != null) && (query.Length > 0))
            {
                _finalUri = _finalUri + "?" + query;
            }

            if (noscript)
            {
                if (_masterKey == null)
                    throw new ArgumentException("For noscript you must also supply the Master Key");
                if ((query != null) && (query.Length > 0))
                {
                    _finalUri = _finalUri + "&noscript=true";
                }
                else
                {
                    _finalUri = _finalUri + "?noscript=true";
                }
            }

            using (var request = (HttpWebRequest)WebRequest.Create(_finalUri))
            {
                //set-up headers
                var headers = new WebHeaderCollection();
                if (_applicationKey != null)
                {
                    headers.Add("X-ZUMO-APPLICATION", _applicationKey);
                }
                if (_masterKey != null)
                {
                    headers.Add("X-ZUMO-MASTER", _masterKey);
                }
                request.Method = "GET";
                request.Headers = headers;
                request.Accept = JsonHeader;

                request.ContentLength = 0;
                request.ContentType = JsonHeader;
                request.UserAgent = "Micro Framework";

                //wait for the response
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    //Check if you are authorized correctly
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return "Please check your Application Key";
                    }

                    //Check if the query was successful
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //Return the string
                        return reader.ReadToEnd();
                    }

                    //Else return the StatusCode and Description
                    return response.StatusCode + " " + response.StatusDescription;
                }
            }
        }
    }
}
