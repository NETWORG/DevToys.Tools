using DevToys.Api;
using Microsoft.Extensions.Logging;
using NETWORG.DevToys.Tools.Helpers;
using NETWORG.DevToys.Tools.Models;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using static DevToys.Api.GUI;

namespace NETWORG.DevToys.Tools;

[Export(typeof(IGuiTool))]
[Name("JsonValueDecoder")]
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",
    IconGlyph = '\ue1d7',
    GroupName = PredefinedCommonToolGroupNames.EncodersDecoders,
    ResourceManagerAssemblyIdentifier = nameof(MyResourceAssemblyIdentifier),
    ResourceManagerBaseName = "NETWORG.DevToys.Tools.JsonValueEncoderDecoderTranslations",
    ShortDisplayTitleResourceName = nameof(JsonValueEncoderDecoderTranslations.ShortDisplayTitle),
    LongDisplayTitleResourceName = nameof(JsonValueEncoderDecoderTranslations.LongDisplayTitle),
    DescriptionResourceName = nameof(JsonValueEncoderDecoderTranslations.Description),
    AccessibleNameResourceName = nameof(JsonValueEncoderDecoderTranslations.AccessibleName))]
internal sealed class JsonValueEncoderDecoderGuiTool : IGuiTool
{
    /// <summary>
    /// Whether the tool should encode or decode a JSON Value.
    /// </summary>
    private static readonly SettingDefinition<EncodingConversion> conversionMode
        = new(
            name: $"{nameof(JsonValueEncoderDecoderGuiTool)}.{nameof(conversionMode)}",
            defaultValue: EncodingConversion.Encode);

    private enum GridRows
    {
        Settings,
        Input,
        Output
    }

    private enum GridColumns
    {
        Stretch
    }

    private readonly DisposableSemaphore _semaphore = new();
    private readonly ILogger _logger;
    private readonly ISettingsProvider _settingsProvider;
    private readonly IUISwitch _conversionModeSwitch = Switch("jsonvalue-conversion-mode-switch");
    private readonly IUIMultiLineTextInput _inputText = MultiLineTextInput("jsonvalue-input-box");
    private readonly IUIMultiLineTextInput _outputText = MultiLineTextInput("jsonvalue-output-box");

    private CancellationTokenSource? _cancellationTokenSource;

    [ImportingConstructor]
    public JsonValueEncoderDecoderGuiTool(ISettingsProvider settingsProvider)
    {
        _logger = this.Log();
        _settingsProvider = settingsProvider;

        switch (_settingsProvider.GetSetting(conversionMode))
        {
            case EncodingConversion.Encode:
                _conversionModeSwitch.On();
                _inputText.AutoWrap();
                _outputText.AlwaysWrap();
                break;

            case EncodingConversion.Decode:
                _inputText.AlwaysWrap();
                _outputText.AutoWrap();
                break;

            default:
                throw new NotSupportedException();
        }
    }

    // For unit tests.
    internal Task? WorkTask { get; private set; }

    public UIToolView View
        => new(
            isScrollable: true,
            Grid()
                .RowLargeSpacing()

                .Rows(
                    (GridRows.Settings, Auto),
                    (GridRows.Input, new UIGridLength(1, UIGridUnitType.Fraction)),
                    (GridRows.Output, new UIGridLength(1, UIGridUnitType.Fraction)))

                .Columns(
                    (GridColumns.Stretch, new UIGridLength(1, UIGridUnitType.Fraction)))

                .Cells(
                    Cell(
                        GridRows.Settings,
                        GridColumns.Stretch,

                        Stack()
                            .Vertical()
                            .SmallSpacing()

                            .WithChildren(
                                Label()
                                    .Text(JsonValueEncoderDecoderTranslations.ShortDisplayTitle),

                                Setting("jsonvalue-conversion-mode-setting")
                                    .Icon("FluentSystemIcons", '\uF18D')
                                    .Title(JsonValueEncoderDecoderTranslations.ConversionTitle)
                                    .Description(JsonValueEncoderDecoderTranslations.ConversionDescription)

                                    .InteractiveElement(
                                        _conversionModeSwitch
                                            .OnText(JsonValueEncoderDecoderTranslations.ConversionEncode)
                                            .OffText(JsonValueEncoderDecoderTranslations.ConversionDecode)
                                            .OnToggle(OnConversionModeChanged)))),

                    Cell(
                        GridRows.Input,
                        GridColumns.Stretch,

                        _inputText
                            .Title(JsonValueEncoderDecoderTranslations.InputTitle)
                            .OnTextChanged(OnInputTextChanged)),

                    Cell(
                        GridRows.Output,
                        GridColumns.Stretch,

                        _outputText
                            .Title(JsonValueEncoderDecoderTranslations.OutputTitle)
                            .ReadOnly()
                            .Extendable())));

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _semaphore.Dispose();
    }

    private void OnConversionModeChanged(bool conversionMode)
    {
        _settingsProvider.SetSetting(JsonValueEncoderDecoderGuiTool.conversionMode, conversionMode ? EncodingConversion.Encode : EncodingConversion.Decode);
        _inputText.Text(_outputText.Text); // This will trigger a conversion.
    }

    private void OnInputTextChanged(string text)
    {
        StartConvert(text);
    }

    private void StartConvert(string text)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();

        WorkTask = ConvertAsync(text, _settingsProvider.GetSetting(conversionMode), _cancellationTokenSource.Token);
    }

    private async Task ConvertAsync(string input, EncodingConversion conversionModeSetting, CancellationToken cancellationToken)
    {
        using (await _semaphore.WaitAsync(cancellationToken))
        {
            await TaskSchedulerAwaiter.SwitchOffMainThreadAsync(cancellationToken);

            _outputText.Text(
                JsonValueHelper.EncodeOrDecode(
                    input,
                    conversionModeSetting,
                    _logger,
                    cancellationToken));
        }
    }
}