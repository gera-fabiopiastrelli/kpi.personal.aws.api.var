using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Amazon.Lambda.Core;

using kpi.personal.aws.api.var.Events;
using kpi.personal.aws.api.var.Services;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace kpi.personal.aws.api.var
{
    public class Function
    {
        /// <summary>
        /// Hash secret
        /// </summary>
        private static readonly String Secret = Environment.GetEnvironmentVariable("EventsSecret");

        /// <summary>
        /// Lambda Function
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(string input, ILambdaContext context)
        {
            try
            {
                // Validades JWT and gets payload
                string payload = ValidateJWT(input);

                // Deserializes the payload
                OrderEventArgs orderEventArgs = JsonConvert.DeserializeObject<OrderEventArgs>(payload);

                // Validates the event
                // Validates the event
                bool cancel;
                if (orderEventArgs.Sub == "OrderIndicatorCycleSet") cancel = false;
                else if (orderEventArgs.Sub == "OrderIndicatorCancelCycleSet") cancel = true;
                else throw new ApplicationException("InvalidEventSubject");

                // Updates kpis
                await OrderService.CreateKpiEventAsync(orderEventArgs, cancel);
            }
            catch (Exception e)
            {
                context.Logger.LogLine(e.Message);
                throw e;
            }
        }

        #region private methods

        private string ValidateJWT(string token)
        {
            // Verifies jwt content
            if (string.IsNullOrEmpty(token))
            {
                throw new ApplicationException("EmptyJWTContent");
            }

            // Gets jwt segments
            string[] segments = token.Split('.');
            if (segments.Length != 3)
            {
                throw new ApplicationException("InvalidJWTContent");
            }

            // Verifies jwt signature
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Secret)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(segments[0] + "." + segments[1]));
                if (Base64UrlEncode(hash) != segments[2])
                {
                    throw new ApplicationException("InvalidJWTSignature");
                }
            }

            // Returns jwt payload
            return Encoding.UTF8.GetString(Base64UrlDecode(segments[1]));
        }

        /// <summary>
        /// From JWT spec
        /// </summary>
        private string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }

        /// <summary>
        /// from JWT spec
        /// </summary>
        private static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new ApplicationException("Illegal base64url string");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }

        #endregion
    }
}
