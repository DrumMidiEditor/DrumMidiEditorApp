﻿using System;

using DrumMidiEditor.pConfig;
using DrumMidiEditor.pGeneralFunction.pUtil;

namespace DrumMidiEditor.pControl;

/// <summary>
/// DmsControlNoteInfo⇒DmsControlMidiMapInfoを通してMIDI再生
/// </summary>
internal class DmsControlNoteInfo : DisposeBaseClass, IComparable, IComparable<DmsControlNoteInfo>
{
	/// <summary>
	/// ノート音量（127基準）
	/// </summary>
	private readonly int _Volume = Config.Media.MidiMaxVolume;

	/// <summary>
	/// MidiMap情報
	/// </summary>
	private DmsControlMidiMapInfo? _MidiMapInfo = null;

	/// <summary>
	/// ノート再生位置の時間（秒）
	/// </summary>
	public double PlaySecond { get; private set; } = 0.0d;

	/// <summary>
	/// NoteOn/Offフラグ (True:NoteOn, False:NoteOff)
	/// </summary>
	public bool NoteOn { get; private set; } = false;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="aPlaySecond">ノート再生位置の時間（秒）</param>
	/// <param name="aVolume">ノート音量（127基準）</param>
	/// <param name="aMidiMapInfo">ノートが属するMidiMap情報</param>
	public DmsControlNoteInfo( double aPlaySecond, int aVolume, bool aNoteOn, ref DmsControlMidiMapInfo aMidiMapInfo )
    {
		PlaySecond		= aPlaySecond;
		_Volume			= aVolume;
		NoteOn			= aNoteOn;
		_MidiMapInfo	= aMidiMapInfo;
	}

	protected override void Dispose( bool aDisposing )
	{
		if ( !_Disposed )
		{
			if ( aDisposing )
			{
				// Dispose managed resources.
				_MidiMapInfo = null;
			}

			// Dispose unmanaged resources.

			_Disposed = true;

			// Note disposing has been done.
			base.Dispose( aDisposing );
		}
	}
	private bool _Disposed = false;

	/// <summary>
	/// ノートの音量＋MidiMapに設定されている音量増減値を加算して再生
	/// </summary>
	public void Play()
	{
		if ( NoteOn )
		{ 
			_MidiMapInfo?.Play( _Volume );
		}
		else
        {
			_MidiMapInfo?.Stop();
		}
	}

	/// <summary>
	/// ノート再生順 並替用
	/// </summary>
	/// <param name="aOther"></param>
	/// <returns></returns>
	public int CompareTo( DmsControlNoteInfo? aOther )
	{
		if ( aOther == null )
		{
			return 1;
		}
		else if ( PlaySecond > aOther.PlaySecond )
		{
			return 1;
		}
		else if ( PlaySecond == aOther.PlaySecond )
		{
			if ( NoteOn == aOther.NoteOn )
			{ 
				return 0;
			}
			return NoteOn ? 1 : -1 ;
		}
		return -1;
	}

	/// <summary>
	/// ノート再生順 並替用
	/// </summary>
	/// <param name="aOther"></param>
	/// <returns></returns>
	public int CompareTo( object? aOther )
	{
		if ( aOther == null )
		{
			return 1;
		}
		if ( GetType() != aOther.GetType() )
		{
                throw new ArgumentException( "Invalid aOther", nameof( aOther ) );
		}
		return CompareTo( aOther as DmsControlNoteInfo );
	}
}
