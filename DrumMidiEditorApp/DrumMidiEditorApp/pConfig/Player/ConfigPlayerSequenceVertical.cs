using System.Text.Json.Serialization;

using DrumMidiEditorApp.pDMS;
using DrumMidiEditorApp.pGeneralFunction.pWinUI;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI;

namespace DrumMidiEditorApp.pConfig;

/// <summary>
/// プレイヤー設定
/// </summary>
public class ConfigPlayerSequenceVertical
{
    #region Bpm

    /// <summary>
    /// BPM行の高さ
    /// </summary>
    [JsonInclude]
    public float BpmHeightSize { get; set; } = 18F;

    /// <summary>
    /// BPMテキスト表示横幅
    /// </summary>
    [JsonInclude]
    public float BpmWidthSize { get; set; } = 40F;

    /// <summary>
    /// BPM描画アイテム
    /// </summary>
    [JsonInclude]
    public FormatRect BpmRect { get; set; } = new()
    {
        BackColor   = Color.FromArgb( 255,   0, 200,   0 ),
        LineColor   = Color.FromArgb( 255,  60,  60,  60 ),
        LineSize    = 0F,
        TextColor   = Color.FromArgb( 255, 100, 200, 100 ),
        TextFormat  = new()
        {
            FontFamily          = "system-ui",
            FontSize            = 14F,
            HorizontalAlignment = CanvasHorizontalAlignment.Left,
            VerticalAlignment   = CanvasVerticalAlignment.Center,
        },
    };

    /// <summary>
    /// 現在のBPM値表示フラグ
    /// </summary>
    [JsonInclude]
    public bool BpmNowDisplay { get; set; } = false;

    /// <summary>
    /// 現在のBPM値描画アイテム
    /// </summary>
    [JsonInclude]
    public FormatRect BpmNowRect { get; set; } = new()
    {
        BackColor   = Color.FromArgb( 160,   0,   0,   0 ),
        LineColor   = Color.FromArgb( 255,  60,  60,  60 ),
        LineSize    = 0.4F,
        TextColor   = Color.FromArgb( 255, 100, 200, 100 ),
        TextFormat  = new()
        {
            FontFamily          = "system-ui",
            FontSize            = 14F,
            HorizontalAlignment = CanvasHorizontalAlignment.Left,
            VerticalAlignment   = CanvasVerticalAlignment.Center,
        },
    };

    #endregion

    #region Measure number

    /// <summary>
    /// 小節番号行の横幅
    /// </summary>
    [JsonInclude]
    public float MeasureNoWidthSize { get; set; } = 22F;

    /// <summary>
    /// 小節番号描画アイテム
    /// </summary>
    [JsonInclude]
    public FormatRect MeasureNoRect { get; set; } = new()
    {
        BackColor   = Color.FromArgb( 255,   0,   0,   0 ),
        LineColor   = Color.FromArgb( 255,  60,  60,  60 ),
        LineSize    = 0F,
        TextColor   = Color.FromArgb( 255, 100, 200, 100 ),
        TextFormat  = new()
        {
            FontFamily          = "system-ui",
            FontSize            = 14F,
            HorizontalAlignment = CanvasHorizontalAlignment.Right,
            VerticalAlignment   = CanvasVerticalAlignment.Bottom,
        },
    };

    #endregion

    #region Header

    /// <summary>
    /// ヘッダーグループ表示
    /// </summary>
    [JsonInclude]
    public bool HeaderGroupOn { get; set; } = false;

    /// <summary>
    /// ヘッダーエフェクト
    /// </summary>
    [JsonInclude]
    public bool HeaderEffectOn { get; set; } = false;

    /// <summary>
    /// ヘッダー高さ
    /// </summary>
    [JsonInclude]
    public float HeaderGroupHeightSize { get; set; } = 30F;

    /// <summary>
    /// ヘッダー高さ
    /// </summary>
    [JsonInclude]
    public float HeaderHeightSize { get; set; } = 100F;

    /// <summary>
    /// ヘッダー高さ
    /// </summary>
    [JsonInclude]
    public float HeaderTotalHeightSize 
        => HeaderGroupOn ? HeaderHeightSize : HeaderHeightSize + HeaderGroupHeightSize;

    /// <summary>
    /// ヘッダー描画アイテム
    /// </summary>
    [JsonInclude]
    public FormatRect HeaderRect { get; set; } = new()
    {
        BackColor   = Color.FromArgb( 160,   0,   0,   0 ),
        LineColor   = Color.FromArgb( 255,  60,  60,  60 ),
        LineSize    = 0.4F,
        TextColor   = Color.FromArgb( 255, 100, 200, 100 ),
        TextFormat  = new()
        {
            FontFamily          = "system-ui",
            FontSize            = 14F,
            HorizontalAlignment = CanvasHorizontalAlignment.Left,
            VerticalAlignment   = CanvasVerticalAlignment.Center,
        },
    };

    #endregion

    #region Note

    /// <summary>
    /// 音量ゼロノート表示フラグ
    /// </summary>
    [JsonInclude]
    public bool NoteVolumeZeroOn { get; set; } = false;

    /// <summary>
    /// 音量サイズ変更フラグ
    /// </summary>
    [JsonInclude]
    public bool NoteVolumeSizeOn { get; set; } = true;

    /// <summary>
    /// ノート高さ
    /// </summary>
    [JsonIgnore]
    public float NoteHeightSize
    {
        get => HeaderGroupOn ? NoteGroupOnHeightSize : NoteGroupOffHeightSize;
        set
        {
            if ( HeaderGroupOn )
            {
                NoteGroupOnHeightSize = value;
            }
            else
            {
                NoteGroupOffHeightSize = value;
            }
        }
    }

    /// <summary>
    /// ノート高さ：グループON時
    /// </summary>
    [JsonInclude]
    public float NoteGroupOnHeightSize { get; set; } = 16F;

    /// <summary>
    /// ノート高さ：グループOFF時
    /// </summary>
    [JsonInclude]
    public float NoteGroupOffHeightSize { get; set; } = 6F;

    /// <summary>
    /// ノート横幅
    /// </summary>
    [JsonIgnore]
    public float NoteWidthSize
    {
        get => HeaderGroupOn ? NoteGroupOnWidthSize : NoteGroupOffWidthSize;
        set
        {
            if ( HeaderGroupOn )
            {
                NoteGroupOnWidthSize = value;
            }
            else
            {
                NoteGroupOffWidthSize = value;
            }
        }
    }

    /// <summary>
    /// ノート横幅：グループON時
    /// </summary>
    [JsonInclude]
    public float NoteGroupOnWidthSize { get; set; } = 16F;

    /// <summary>
    /// ノート横幅：グループOFF時
    /// </summary>
    [JsonInclude]
    public float NoteGroupOffWidthSize { get; set; } = 6F;

    /// <summary>
    /// ノート間隔：横
    /// </summary>
    [JsonInclude]
    public float NoteTermWidthSize
    {
        get => HeaderGroupOn ? NoteGroupOnTermWidthSize : NoteGroupOffTermWidthSize ;
        set
        {
            if ( HeaderGroupOn )
            {
                NoteGroupOnTermWidthSize = value;
            }
            else
            {
                NoteGroupOffTermWidthSize = value;
            }
        }
    }

    /// <summary>
    /// ノート間隔：横：グループON時
    /// </summary>
    [JsonInclude]
    public float NoteGroupOnTermWidthSize { get; set; } = 10F;

    /// <summary>
    /// ノート間隔：横：グループOFF時
    /// </summary>
    [JsonInclude]
    public float NoteGroupOffTermWidthSize { get; set; } = 6F;

    /// <summary>
    /// ノート間隔：縦
    /// </summary>
    [JsonInclude]
    public float NoteTermHeightSize { get; set; } = 2F;

    #endregion

    #region Line

    /// <summary>
    /// 小節128分間隔の線ペン
    /// </summary>
    [JsonInclude]
    public FormatLine SheetMeasure128Line { get; set; } = new()
    {
        LineColor   = Color.FromArgb( 255, 80, 80, 80 ),
        LineSize    = 1.0F,
    };

    /// <summary>
    /// 小節64分間隔の線ペン
    /// </summary>
    [JsonInclude]
    public FormatLine SheetMeasure064Line { get; set; } = new()
    {
        LineColor   = Color.FromArgb( 255, 70, 70, 70 ),
        LineSize    = 0.1F,
    };

    /// <summary>
    /// 小節32分間隔の線ペン
    /// </summary>
    [JsonInclude]
    public FormatLine SheetMeasure032Line { get; set; } = new()
    {
        LineColor   = Color.FromArgb( 255, 60, 60, 60 ),
        LineSize    = 0.0F,
    };

    /// <summary>
    /// 小節16分間隔の線ペン
    /// </summary>
    [JsonInclude]
    public FormatLine SheetMeasure016Line { get; set; } = new()
    {
        LineColor   = Color.FromArgb( 255, 50, 50, 50 ),
        LineSize    = 0.0F,
    };

    /// <summary>
    /// 小節8分間隔の線ペン
    /// </summary>
    [JsonInclude]
    public FormatLine SheetMeasure008Line { get; set; } = new()
    {
        LineColor   = Color.FromArgb( 255, 40, 40, 40 ),
        LineSize    = 0.0F,
    };

    /// <summary>
    /// 小節4分間隔の線ペン
    /// </summary>
    [JsonInclude]
    public FormatLine SheetMeasure004Line { get; set; } = new()
    {
        LineColor   = Color.FromArgb( 255, 30, 30, 30 ),
        LineSize    = 0.0F,
    };

    /// <summary>
    /// 小節1分間隔の線ペン
    /// </summary>
    [JsonInclude]
    public FormatLine SheetMeasure001Line { get; set; } = new()
    {
        LineColor   = Color.FromArgb( 255, 20, 20, 20 ),
        LineSize    = 0.0F,
    };

    #endregion

    /// <summary>
    /// スコア最大横幅
    /// </summary>
    public float ScoreMaxWidth 
        => NoteTermWidthSize * ( HeaderGroupOn ? DMS.SCORE.EditMidiMapSet.DisplayGroupCount : DMS.SCORE.EditMidiMapSet.DisplayMidiMapAllCount ) ;

    /// <summary>
    /// スコア最大高さ
    /// </summary>
    public float ScoreMaxHeight => NoteTermHeightSize * Config.System.NoteCount;

    /// <summary>
    /// １小節の横幅
    /// </summary>
    public float MeasureSize => NoteTermHeightSize * Config.System.MeasureNoteNumber;
}
