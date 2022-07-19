﻿using DrumMidiEditorApp.pConfig;
using DrumMidiEditorApp.pDMS;
using DrumMidiEditorApp.pGeneralFunction.pUtil;

namespace DrumMidiEditorApp.pResume;

/// <summary>
/// レジューム：BPM設定
/// </summary>
internal class ResumeEditAddBpm : DisposeBaseClass, IResume
{
    /// <summary>
    /// 変更前 BPM情報
    /// </summary>
    private InfoBpm? _InfoAft;

    /// <summary>
    /// 変更後 BPM情報
    /// </summary>
    private InfoBpm? _InfoBef;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="aInfoBef">変更前 BPM情報</param>
    /// <param name="aInfoAft">変更後 BPM情報</param>
    public ResumeEditAddBpm( InfoBpm? aInfoBef, InfoBpm aInfoAft )
    {
        _InfoBef = aInfoBef;
        _InfoAft = aInfoAft;
    }

	protected override void Dispose( bool aDisposing )
	{
		if ( !_Disposed )
		{
			if ( aDisposing )
			{
                // Dispose managed resources.
                _InfoAft = null;
                _InfoBef = null;
            }

            // Dispose unmanaged resources.

            _Disposed = true;

			// Note disposing has been done.
			base.Dispose( aDisposing );
		}
	}
    private bool _Disposed = false;

    public void Undo()
    {
        if ( _InfoBef != null )
        {
			DMS.SCORE.SysChannel.AddBpm( _InfoBef );

            Update( _InfoBef.MeasureNo );
        }
        else if( _InfoAft != null )
        {
			DMS.SCORE.SysChannel.RemoveBpm( _InfoAft );

            Update( _InfoAft.MeasureNo );
        }
    }

    public void Redo()
    {
        if( _InfoAft == null )
        {
            return;
        }

		DMS.SCORE.SysChannel.AddBpm( _InfoAft );

        Update( _InfoAft.MeasureNo );
    }

    /// <summary>
    /// Undo/Redo共通処理
    /// </summary>
    /// <param name="aMeasureNo">小節番号</param>
    private static void Update( int aMeasureNo )
    {
        Config.EventEditBpm( aMeasureNo );
    }
}
