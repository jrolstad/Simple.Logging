using System;
using System.Collections.Generic;
using System.Globalization;
using Amazon.SimpleDB.Model;
using System.Linq;

namespace Simple.Logging.Aws.Mappers
{
    public class LogMessageMapper:IMapper<LogMessage,PutAttributesRequest>
    {
        public PutAttributesRequest Map(LogMessage toMap)
        {
            var attributes = MapAttributes(toMap)
                .ToArray();

            var request = new PutAttributesRequest()
                .WithDomainName(toMap.LogDomainName)
                .WithItemName(toMap.Identifier)
                .WithAttribute(attributes);

            return request;
        }

        private IEnumerable<ReplaceableAttribute> MapAttributes(LogMessage toMap)
        {
            yield return new ReplaceableAttribute().WithName("LogName").WithValue(toMap.LogName);


            yield return new ReplaceableAttribute().WithName("Level").WithValue(toMap.Level.ToString());

            const int maximumMessageSize = 512;
            var messageSize = Math.Min(toMap.Message.Length, maximumMessageSize);
            var trimmedMessage = toMap.Message.Substring(0, messageSize).Trim();

            yield return new ReplaceableAttribute().WithName("Message").WithValue(trimmedMessage);

            yield return new ReplaceableAttribute().WithName("CreatedAt").WithValue(toMap.CreatedAt.ToString(CultureInfo.CurrentCulture));

            yield return new ReplaceableAttribute().WithName("MachineName").WithValue(toMap.MachineName);

            yield return new ReplaceableAttribute().WithName("UserName").WithValue(toMap.UserName);
        }
    }
}