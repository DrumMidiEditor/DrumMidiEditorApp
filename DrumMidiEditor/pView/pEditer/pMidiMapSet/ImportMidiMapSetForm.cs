using System;
using System.Collections.Generic;
using System.Windows.Forms;

using DrumMidiEditor.pDMS;
using DrumMidiEditor.pGeneralFunction.pLog;

namespace DrumMidiEditor.pView.pEditer.pMidiMapSet;

/// <summary>
/// ドラムセットインポートフォーム
/// </summary>
public partial class ImportMidiMapSetForm : Form
{
	/// <summary>
	/// インポート実施フラグ
	/// </summary>
    private bool _ImportFlag = false;

	/// <summary>
	/// コンストラクタ
	/// </summary>
    public ImportMidiMapSetForm()
    {
        InitializeComponent();

        #region ToolTip

		ToolTip.SetToolTip( ImportButton	, "Update MidiMapSet for this channel" );
		ToolTip.SetToolTip( CxlButton		, "Cancel" );

		#endregion
    }

	/// <summary>
	/// インポートフォーム表示
	/// </summary>
	/// <param name="aMidiMapSet">インポートするMidiMapSet</param>
	/// <returns>True:インポート実施、False:キャンセル</returns>
	public bool Display( MidiMapSet aMidiMapSet )
    {
		_ImportFlag = false;

		SuspendLayout();

		try
		{
			#region 変換後のMidiMapSetから選択
			{
				ImportMidiMapSetGridView.Rows.Clear();

				foreach ( var group in aMidiMapSet.MidiMapGroups )
				{
					foreach ( var midiMap in group.MidiMaps )
					{
						var name = aMidiMapSet.GetGroupAndMidiMapName( midiMap.MidiMapKey );

						ImportMidiMapSetGridView.Rows.Add( $"{midiMap.MidiMapKey,-3} {name,-30}" ); 
					}
				}
			}
			#endregion

			#region 変換後のMidiMapSetから選択
			{
				MidiMapAssignColumn.Items.Clear();
				MidiMapAssignColumn.Items.Add( String.Empty );

				foreach ( var group in aMidiMapSet.MidiMapGroups )
				{
					foreach ( var midiMap in group.MidiMaps )
					{
						var name = aMidiMapSet.GetGroupAndMidiMapName( midiMap.MidiMapKey );

						MidiMapAssignColumn.Items.Add( $"{midiMap.MidiMapKey,-3} {name,-30}" ); 
					}
				}
			}
			#endregion

			#region 変換前のMidiMapSetから使用されているMidiMapのみ抽出
			{
				ConvertDataGridView.Rows.Clear();

				var items = DMS.SCORE.EditChannel.GetNumberOfNoteUsingMidiMap();

				foreach ( var midiMap in DMS.SCORE.EditMidiMapSet.MidiMaps )
				{
					if ( !items.TryGetValue( midiMap.MidiMapKey, out var cnt ) )
					{
						continue;
					}

					var name_o = DMS.SCORE.EditMidiMapSet.GetGroupAndMidiMapName( midiMap.MidiMapKey );

					var row_index = ConvertDataGridView.Rows.Add
						(
							$"{midiMap.MidiMapKey,-3} {name_o,-30} [{cnt,4}]",
							String.Empty
						);

					var midiMapKey_n = aMidiMapSet.GetMidiMapKeyForMatchMidi( midiMap.Midi );

					if ( midiMapKey_n != -1 )
                    {
						var name_n = aMidiMapSet.GetGroupAndMidiMapName( midiMapKey_n );

						var c = ConvertDataGridView[ MidiMapAssignColumn.Index, row_index ];
						c.Value = $"{midiMapKey_n,-3} {name_n,-30}";
					}
				}

				items.Clear();
			}
			#endregion

			Location = Cursor.Position;
			ShowDialog();
			Owner.Focus();
		}
		catch ( Exception e )
		{
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
		}

		ResumeLayout( false );

		return _ImportFlag;
    }

	/// <summary>
	/// インポート実行
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="ev"></param>
    private void ImportButton_Click( object sender, EventArgs ev )
    {
		try
		{
			var changeKey = new Dictionary<int,int>();

			foreach ( DataGridViewRow row in ConvertDataGridView.Rows )
			{
				var midiMapKeyBef = row.Cells[ MidiMapUsedColumn.Index  ].Value.ToString()?.Split(' ')[ 0 ];
				var midiMapKeyAft = row.Cells[ MidiMapAssignColumn.Index ].Value.ToString()?.Split(' ')[ 0 ];

				if ( int.TryParse( midiMapKeyAft, out _ ) )
                {
					changeKey.Add( Convert.ToInt32( midiMapKeyBef ), Convert.ToInt32( midiMapKeyAft ) );
				}
			}

			foreach ( var item in changeKey )
            {
				DMS.SCORE.EditChannel.KeyChange( item.Key, -item.Value );
			}

			foreach ( var item in changeKey )
			{
				DMS.SCORE.EditChannel.KeyChange( -item.Value, item.Value );
			}

			_ImportFlag = true;
			Hide();
		}
		catch ( Exception e )
		{
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
		}
	}

	/// <summary>
	/// キャンセル
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="ev"></param>
	private void CxlButton_Click( object sender, EventArgs ev )
	{
		try
		{
			Hide();
		}
		catch ( Exception e )
		{
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
		}
	}

	/// <summary>
	/// セル選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="ev"></param>
    private void ConvertDataGridView_CellEnter( object sender, DataGridViewCellEventArgs ev )
    {
		try
		{
			if  ( ConvertDataGridView[ ev.ColumnIndex, ev.RowIndex ].GetType().Equals( typeof( DataGridViewComboBoxCell ) ) )
			{
				ConvertDataGridView.BeginEdit( false );
				( (DataGridViewComboBoxEditingControl)ConvertDataGridView.EditingControl ).DroppedDown = true;
			}
		}
		catch ( Exception e )
		{
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
		}
    }
}
