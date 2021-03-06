using Windows.UI;

using DrumMidiEditorApp.pConfig;
using DrumMidiEditorApp.pGeneralFunction.pUtil;

namespace DrumMidiEditorApp.pDMS;

/// <summary>
/// MidiMap
/// </summary>
public class MidiMap : DisposeBaseClass
{
    /// <summary>
    /// 所属するMidiMapGroup
    /// </summary>
    public MidiMapGroup? Group { get; set; } = null;

    /// <summary>
    /// 表示設定
    /// </summary>
    public bool Display { get; set; } = Config.System.DefaultMidiMapDisplay;

    /// <summary>
    /// MidiMapキー
    /// </summary>
    public int MidiMapKey { get; set; } = Config.System.MidiMapKeyNotSelect;

    /// <summary>
    /// ラベル名称
    /// </summary>
    public string MidiMapName { get; set; } = Config.System.DefaultMidiMapName;

    /// <summary>
    /// MIDiノート番号
    /// </summary>
    public byte Midi { get; set; } = Config.System.DefaultMidiMapMidi;

    /// <summary>
    /// 音量増減値
    /// </summary>
    public int VolumeAdd { get; set; } = Config.System.DefaultMidiMapVolumeAdd;

    /// <summary>
    /// ノートの色
    /// </summary>
    public Color Color { get; set; } = Config.System.DefaultMidiMapColor;

    /// <summary>
    /// 選択状態
    /// </summary>
    public bool Selected { get; set; } = false;

    /// <summary>
    /// 音階名称（ピッチ＋音階）
    /// </summary>
    public string Scale { get; set; } = Config.System.DefaultMidiMapScale;

    protected override void Dispose( bool aDisposing )
	{
		if ( !_Disposed )
		{
			if ( aDisposing )
			{
				// Dispose managed resources.
				Group = null;
			}

			// Dispose unmanaged resources.

			_Disposed = true;

			// Note disposing has been done.
			base.Dispose( aDisposing );
		}
	}
    private bool _Disposed = false;

    /// <summary>
    /// 音量増減値を取得
    /// （MidiMapの音量増減＋MidiMapGroupの音量増減）
    /// </summary>
    public int VolumeAddIncludeGroup => VolumeAdd + Group?.VolumeAdd ?? 0;

    /// <summary>
    /// MidiMapを複製。
    /// 但し、選択状態は初期化されます。
    /// また、グループの情報はNULLとなるので複製後 再設定が必要です。
    /// </summary>
    public MidiMap Clone()
    {
        return new()
        {
        //  Group       = this.Group,
            Display     = this.Display,
            MidiMapKey  = this.MidiMapKey,
            MidiMapName = this.MidiMapName,
            VolumeAdd   = this.VolumeAdd,
            Midi        = this.Midi,
            Color       = this.Color,
        //  Selected    = this.Selected,
            Scale       = this.Scale,
        };
    }
}
