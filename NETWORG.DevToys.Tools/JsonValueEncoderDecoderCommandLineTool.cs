using DevToys.Api;
using Microsoft.Extensions.Logging;
using NETWORG.DevToys.Tools.Helpers;
using NETWORG.DevToys.Tools.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NETWORG.DevToys.Tools
{
    [Export(typeof(ICommandLineTool))]
    [Name("JsonValueEncoderDecoder")]
    [CommandName(
    Name = "jsonvalue",
    ResourceManagerBaseName = "NETWORG.DevToys.Tools.JsonValueEncoderDecoderTranslations",
    DescriptionResourceName = nameof(JsonValueEncoderDecoderTranslations.Description))]
    internal sealed class JsonValueEncoderDecoderCommandLineTool : ICommandLineTool
    {
        [CommandLineOption(
            Name = "input",
            Alias = "i",
            IsRequired = true,
            DescriptionResourceName = nameof(JsonValueEncoderDecoderTranslations.InputOptionDescription))]
        private string? Input { get; set; }

        [CommandLineOption(
            Name = "conversion",
            Alias = "c",
            DescriptionResourceName = nameof(JsonValueEncoderDecoderTranslations.ConversionOptionDescription))]
        private EncodingConversion EncodingConversionMode { get; set; } = EncodingConversion.Encode;

        public ValueTask<int> InvokeAsync(ILogger logger, CancellationToken cancellationToken)
        {
            Console.WriteLine(
                JsonValueHelper.EncodeOrDecode(
                    Input,
                    EncodingConversionMode,
                    logger,
                    cancellationToken));

            return new ValueTask<int>(0);
        }
    }
}
