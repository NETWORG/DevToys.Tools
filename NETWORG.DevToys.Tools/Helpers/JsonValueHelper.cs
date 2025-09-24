using Microsoft.Extensions.Logging;
using NETWORG.DevToys.Tools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NETWORG.DevToys.Tools.Helpers
{
    internal static class JsonValueHelper
    {
        internal static string EncodeOrDecode(string? input, EncodingConversion conversionMode, ILogger logger, CancellationToken cancellationToken)
        {
            string conversionResult
                = conversionMode switch
                {
                    EncodingConversion.Encode
                        => JsonSerializer.Serialize(input).Trim('"'),

                    EncodingConversion.Decode
                        => JsonSerializer.Deserialize<string>($"\"{input}\""),

                    _ => throw new NotSupportedException(),
                };

            cancellationToken.ThrowIfCancellationRequested();

            return conversionResult;
        }
    }
}
