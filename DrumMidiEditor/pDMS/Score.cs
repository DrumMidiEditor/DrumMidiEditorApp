using System.Collections.Generic;

using DrumMidiEditor.pConfig;
using DrumMidiEditor.pGeneralFunction.pUtil;

namespace DrumMidiEditor.pDMS;

/// <summary>
/// スコア情報
/// </summary>
public class Score : DisposeBaseClass
{
    /// <summary>
    /// ファイルパス
    /// </summary>
    public GeneralPath FilePath { get; set; } = Config.System.DefaultScoreFilePath;

    /// <summary>
    /// メモ情報
    /// </summary>
    public string Info { get; set; } = Config.System.DefaultScoreInfo;

    /// <summary>
    /// BGMファイルパス
    /// </summary>
    public GeneralPath BgmFilePath { get; set; } = Config.System.DefaultBgmFilePath;

    /// <summary>
    /// BGM再生開始位置（秒）
    /// </summary>
    public double BgmReproducePosition { get; set; } = Config.System.DefaultBgmReproducePosition;

    /// <summary>
    /// ベースBPM
    /// </summary>
    public double Bpm { get; set; } = Config.System.DefaultBpm;

    /// <summary>
    /// BGM音量(100基準)
    /// </summary>
    public int BgmVolume { get; set; } = Config.System.DefaultBgmVolume;

    /// <summary>
    /// システムチャンネル
    /// </summary>
    public ChannelSys SysChannel { get; private set; } = new();

    /// <summary>
    /// チャンネルリスト（チャンネル番号(0-15)、チャンネル情報）
    /// </summary>
    public Dictionary<byte, Channel> Channels { get; private set; } = new();

    /// <summary>
    /// 編集中のチャンネル番号
    /// </summary>
    public byte EditChannelNo { get; set; } = 0;

    /// <summary>
    /// 編集中のチャンネル情報
    /// </summary>
    public Channel EditChannel => Channels[ EditChannelNo ];

    /// <summary>
    /// 編集中のチャンネル情報のMidiMapSet
    /// </summary>
    public MidiMapSet EditMidiMapSet
    {
        get => Channels[ EditChannelNo ].MidiMapSet;
        set => Channels[ EditChannelNo ].MidiMapSet = value;  
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Score()
    {
        for ( byte channel_no = Config.Media.ChannelMinNo; channel_no <= Config.Media.ChannelMaxNo; channel_no++ )
        {
            Channels.Add( channel_no, new( channel_no ) );
        }
    }

	protected override void Dispose( bool aDisposing )
	{
		if ( !_Disposed )
		{
			if ( aDisposing )
			{
                // Dispose managed resources.
                ClearChannel();
			}

			// Dispose unmanaged resources.

			_Disposed = true;

			// Note disposing has been done.
			base.Dispose( aDisposing );
		}
	}
    private bool _Disposed = false;

    #region Function

    /// <summary>
    /// データクリア（チャンネル情報）
    /// </summary>
    public void ClearChannel()
	{
        SysChannel.ClearAll();

		foreach ( var channel in Channels.Values )
		{
            channel.ClearAll();
		}
    }

    /// <summary>
    /// NOTE/BPMが存在する小節番号最大値
    /// </summary>
    public int GetMaxMeasureNo()
    {
        var max = SysChannel.MaxMeasureNo;

        foreach ( var channel in Channels.Values )
        {
            if ( channel.MaxMeasureNo > max )
            {
                max = channel.MaxMeasureNo;
            }
        }

        return max; 
    }

    /// <summary>
    /// 全チャンネルのMidiMapSet更新処理を実行
    /// </summary>
    public void UpdateInfoAllMidiMaps()
    {
        foreach ( var channel in Channels.Values )
        {
            channel.MidiMapSet.UpdateInfo();
        }
    }

    /// <summary>
    /// MidiMapSetを一つ取得。
    /// チェンネルを昇順で検索し、MidiMapGroupが１つでも設定されている情報を対象とします。
    /// </summary>
    /// <returns></returns>
    public MidiMapSet? GetFirstMidiMapSet()
    {
        foreach ( var channel in Channels.Values )
        {
            if ( channel.MidiMapSet.MidiMapGroups.Count > 0 )
            {
                return channel.MidiMapSet;
            }
        }
        return null;
    }

    #endregion
}
