﻿using System;
using System.Collections.Generic;

using DrumMidiEditorApp.pConfig;
using DrumMidiEditorApp.pDMS;

namespace DrumMidiEditorApp.pControl;

/// <summary>
/// ノート位置の再生時間やＢＰＭ値 計算表
/// </summary>
internal class TimeTable
{
	/// <summary>
	/// 排他制御用ロックオブジェクト
	/// </summary>
	public readonly object LockObj = new();

	/// <summary>
	/// TimeTable更新フラグ
	/// </summary>
	private bool _UpdateFlag = true;

	/// <summary>
	/// TimeTable
	/// </summary>
	private double[] _TimeTables = new double[ 10 ];

	/// <summary>
	/// ノート位置（絶対値）、Bpm入力値
	/// </summary>
	private readonly Dictionary<int, double> _BpmDic = new();

	/// <summary>
	/// ベースBPM
	/// </summary>
	private double _BaseBpm = Config.System.DefaultBpm;

	/// <summary>
	/// TimeTableへのアクセス。＜排他制御なし＞
	/// 指定したノート位置（絶対値）の時間（秒）を取得
	/// </summary>
	/// <param name="aIndex">ノート位置（絶対値）</param>
	/// <returns>時間（秒）</returns>
	public double this[ int aIndex ] => _TimeTables[ aIndex ]; 

	/// <summary>
	/// TimeTable末尾の時間（秒）を取得
	/// </summary>
	public double EndTime 
	{
		get
        {
			lock ( LockObj ) 
			{ 
				return _TimeTables[ Config.System.NoteCount ];
			}
        }
	}        

	/// <summary>
	/// 1ノート辺りの時間（秒）を取得。
	/// （Musicタブで入力したBPM値基準）
	/// </summary>
	public double NoteTime
	{ 
		get
        {
			lock ( LockObj )
			{ 
				return 240.0d / ( _BaseBpm * Config.System.MeasureNoteNumber );
			}
		}
	}

	/// <summary>
	/// 指定したノート位置（絶対値）のBPM値を取得
	/// </summary>
	/// <param name="aAbsoluteNotePos">ノート位置（絶対値）</param>
	/// <returns>BPM値</returns>
	public double GetBpm( int aAbsoluteNotePos )
    {
		double bpm;

		lock ( LockObj )
		{ 
			bpm = _BpmDic[ 0 ];

			var pos = 0;

			foreach ( var item in _BpmDic )
			{
				if ( item.Key == aAbsoluteNotePos )
				{
					bpm = item.Value;
					break;
				}
				else if ( item.Key < aAbsoluteNotePos )
				{
					if ( item.Key > pos )
					{
						bpm = item.Value;
						pos = item.Key;
					}
				}
			}
		}
		return bpm;
    }

	/// <summary>
	/// 指定した時間のノート位置（絶対値）を取得
	/// </summary>
	/// <param name="aCurrentTime">秒数</param>
	/// <returns>ノート位置（絶対値）0～</returns>
	public int SearchPosition( double aCurrentTime )
    {
		lock ( LockObj )
		{ 
			for ( int measure_no = 0; measure_no <= Config.System.MeasureMaxNumber; measure_no++ )
			{
				var note_pos_s = measure_no * Config.System.MeasureNoteNumber;

				if ( _TimeTables[ note_pos_s ] > aCurrentTime )
				{
					for( int note_pos = 1; note_pos <= Config.System.MeasureNoteNumber; note_pos++ )
					{
						if ( _TimeTables[ note_pos_s - note_pos ] == aCurrentTime )
						{
							return note_pos_s - note_pos;
						}
						if ( _TimeTables[ note_pos_s - note_pos ] < aCurrentTime )
						{
							return note_pos_s - note_pos + 1;
						}
					}
				}
			}
		}
		return 0;
	}

	/// <summary>
	/// TimeTable更新フラグを立てます。
	/// TimeTable.Update実行時にTimeTableを更新します。
	/// </summary>
	public void Refresh() => _UpdateFlag = true;

	/// <summary>
	/// TimeTableを更新します。
	/// Musicタブで入力したBPM値とEditタブ画面で入力したBPM値を元に
	/// 各ノート位置の再生時間を算出します。
	/// </summary>
	public void Update()
	{
		if ( !_UpdateFlag )
		{
			return;
		}
		_UpdateFlag = false;

		lock ( LockObj )
		{
			_BaseBpm = DMS.SCORE.Bpm;

			_BpmDic.Clear();
			_BpmDic[ 0 ] = _BaseBpm;

			_TimeTables			= new double[ Config.System.NoteCount + 2 ];
			_TimeTables[ 0 ]	= 0;

			var note_time = NoteTime;

			int indexCnt = 0;

			for ( int measure_no = 0; measure_no <= Config.System.MeasureMaxNumber; measure_no++ )
			{
				var measure = DMS.SCORE.SysChannel.GetMeasure( measure_no );

                if ( measure == null )
                {
				    for( int note_pos = 0; note_pos < Config.System.MeasureNoteNumber; note_pos++ )
				    {
						_TimeTables[ indexCnt + 1 ] = note_time + _TimeTables[ indexCnt ];

						indexCnt++;
				    }
                }
                else
                {
				    for( int note_pos = 0; note_pos < Config.System.MeasureNoteNumber; note_pos++ )
				    {
                        if ( measure.BpmLine.InfoStates.TryGetValue( note_pos, out var info ) )
                        {
                            note_time = 240.0d / ( info.Bpm * Config.System.MeasureNoteNumber );

							_BpmDic[ info.AbsoluteNotePos ] = info.Bpm;
                        }

						_TimeTables[ indexCnt + 1 ] = note_time + _TimeTables[ indexCnt ];

						indexCnt++;
				    }
                }
			}

			for ( int index = 0; index < indexCnt; index++ )
			{
				_TimeTables[ index ] = Math.Round( _TimeTables[ index ], 8 );
			}
		}
	}
}
