﻿using DrumMidiClassLibrary.pModel;
using DrumMidiClassLibrary.pUtil;

using DrumMidiEditorApp.pEvent;

namespace DrumMidiEditorApp.pResume;

/// <summary>
/// レジューム：MidiMap選択
/// </summary>
internal class ResumeEditSelectMidiMap : DisposeBaseClass, IResume
{
    /// <summary>
    /// 編集対象のMidiMap
    /// </summary>
    private MidiMap? _MidiMap;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="aMidiMap">編集対象のMidiMap</param>
    public ResumeEditSelectMidiMap( MidiMap aMidiMap )
    {
        _MidiMap = aMidiMap;
    }

	protected override void Dispose( bool aDisposing )
	{
		if ( !_Disposed )
		{
			if ( aDisposing )
			{
				// Dispose managed resources.
				_MidiMap = null;
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
        Update();
    }

    public void Redo()
    {
        Update();
    }

    /// <summary>
    /// Undo/Redo共通処理
    /// </summary>
    private void Update()
    {
        if ( _MidiMap == null )
        {
            return;
        }
        _MidiMap.Selected = !_MidiMap.Selected;

        EventManage.EventEditUpdateDisplayNoteVolume();
    }
}
