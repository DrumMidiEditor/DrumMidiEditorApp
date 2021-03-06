using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using DrumMidiEditor.pConfig;
using DrumMidiEditor.pControl;
using DrumMidiEditor.pDMS;
using DrumMidiEditor.pGeneralFunction.pLog;
using DrumMidiEditor.pGeneralFunction.pUtil;
using DrumMidiEditor.pIO.pJson;
using DrumMidiEditor.pIO.pScore;

namespace DrumMidiEditor.pIO;

/// <summary>
/// ファイル入出力
/// </summary>
public static class FileIO
{
    #region Config I/O

    /// <summary>
    /// 設定ファイル読込
    /// </summary>
    /// <returns>Trueのみ</returns>
    public static bool LoadConfig()
    {
		using var _ = new LogBlock( Log.GetThisMethodName );

#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します
        var path = new GeneralPath( Config.System.FolderConfig );
#pragma warning restore IDE0017 // オブジェクトの初期化を簡略化します

        // ConfigSystem
        path.FileName = Config.System.FileNameConfigSystem;
        Config.System = LoadConfig<ConfigSystem>( path ) ?? Config.System ;

        // ConfigMedia
        path.FileName = Config.System.FileNameConfigMedia;
        Config.Media = LoadConfig<ConfigMedia>( path ) ?? Config.Media ;

        // ConfigEditer
        path.FileName = Config.System.FileNameConfigEditer;
        Config.Editer = LoadConfig<ConfigEditer>( path ) ?? Config.Editer ;

        // ConfigPlayer
        path.FileName = Config.System.FileNameConfigPlayer;
        Config.Player = LoadConfig<ConfigPlayer>( path ) ?? Config.Player ;

        // ConfigScore
        path.FileName = Config.System.FileNameConfigScore;
        Config.Score = LoadConfig<ConfigScore>( path ) ?? Config.Score ;

        // ConfigEqualizer
        path.FileName = Config.System.FileNameConfigEqualizer;
        Config.Equalizer = LoadConfig<ConfigEqualizer>( path ) ?? Config.Equalizer ;

        // ConfigScale
        path.FileName = Config.System.FileNameConfigScale;
        Config.Scale = LoadConfig<ConfigScale>( path ) ?? Config.Scale;

        return true;
    }

    /// <summary>
    /// コンフィグファイル読込
    /// </summary>
    /// <typeparam name="T">Configクラス</typeparam>
    /// <param name="aGeneralPath">読込ファイルパス</param>
    /// <returns>読込済みConfigオブジェクト</returns>
    private static T? LoadConfig<T>( GeneralPath aGeneralPath ) where T : class
    {
        T? config = null;

        try
        {
            config = JsonIO.LoadFile<T>( aGeneralPath );

            Log.Info( $"Succeeded in reading [{aGeneralPath.AbsoulteFilePath}]" );
        }
        catch ( Exception e )
        {
            Log.Error( $"Failed to read [{aGeneralPath.AbsoulteFilePath}]" );
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
        }
        return config;
    }

    /// <summary>
    /// 設定ファイル保存
    /// </summary>
    /// <returns>Trueのみ</returns>
    public static bool SaveConfig()
    {
		using var _ = new LogBlock( Log.GetThisMethodName );

#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します
        var path = new GeneralPath( Config.System.FolderConfig );
#pragma warning restore IDE0017 // オブジェクトの初期化を簡略化します

        // ConfigSystem
        path.FileName = Config.System.FileNameConfigSystem;
        SaveConfig<ConfigSystem>( path, Config.System );

        // ConfigMedia
        path.FileName = Config.System.FileNameConfigMedia;
        SaveConfig<ConfigMedia>( path, Config.Media );

        // ConfigEditer
        path.FileName = Config.System.FileNameConfigEditer;
        SaveConfig<ConfigEditer>( path, Config.Editer );

        // ConfigPlayer
        path.FileName = Config.System.FileNameConfigPlayer;
        SaveConfig<ConfigPlayer>( path, Config.Player );

        // ConfigScore
        path.FileName = Config.System.FileNameConfigScore;
        SaveConfig<ConfigScore>( path, Config.Score );

        // ConfigEqualizer
        path.FileName = Config.System.FileNameConfigEqualizer;
        SaveConfig<ConfigEqualizer>( path, Config.Equalizer );

        // ConfigScale
        path.FileName = Config.System.FileNameConfigScale;
        SaveConfig<ConfigScale>( path, Config.Scale );

        return true;
    }

    /// <summary>
    /// コンフィグファイル出力
    /// </summary>
    /// <typeparam name="T">Configクラス</typeparam>
    /// <param name="aGeneralPath">出力ファイルパス</param>
    /// <param name="aGeneralPath">出力Configオブジェクト</param>
    private static void SaveConfig<T>( GeneralPath aGeneralPath, T aConfig ) where T : class
    {
        try
        {
            JsonIO.SaveFile<T>( aConfig, aGeneralPath );

            Log.Info( $"Succeeded in writing [{aGeneralPath.AbsoulteFilePath}]" );
        }
        catch ( Exception e )
        {
            Log.Error( $"Failed to write [{aGeneralPath.AbsoulteFilePath}]" );
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
        }
    }

    #endregion

    /// <summary>
    /// スコア保存
    /// </summary>
    /// <param name="aFilePath">スコアファイルパス</param>
    /// <param name="aScore">出力スコア</param>
    /// <returns>True:読込成功、False:読込失敗</returns>
    public static bool LoadScore( GeneralPath aFilePath, out Score aScore )
    {
		using var _ = new LogBlock( Log.GetThisMethodName );

        aScore = new Score();

        try
        {
            var ext = aFilePath.Extension.ToLower();

            if ( ext.Equals( ".dms" ) )
            {
                ScoreIO.LoadFile( aFilePath, out aScore );
            }
            else if ( ext.Equals( ".mid" ) )
            {
                ScoreIO.LoadMidiFile( aFilePath, out aScore );
            }
            else if ( ext.Equals( ".dtx" ) )
            {
                // not recommended
                ScoreIO.LoadDtxManiaFile( aFilePath, out aScore );
            }
            else
            {
                throw new NotSupportedException( $"Extension {ext} is not supported" );
            }

            Log.Info( $"Succeeded in reading [{aFilePath.AbsoulteFilePath}]" );
        }
        catch ( Exception e )
        {
            Log.Error( $"Failed to read [{aFilePath.AbsoulteFilePath}]" );
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
            return false;
        }
        return true;
    }

    /// <summary>
    /// スコア保存
    /// </summary>
    /// <param name="aFilePath">出力先ファイルパス</param>
    /// <param name="aScore">保存スコア</param>
    /// <returns>True:保存成功、False:保存失敗</returns>
    public static bool SaveScore( GeneralPath aFilePath, Score aScore )
    {
		using var _ = new LogBlock( Log.GetThisMethodName );

        try
        {
            var ext = aFilePath.Extension.ToLower();

            if ( ext.ToLower().Equals( ".dms" ) )
            {
                ScoreIO.SaveFile( aFilePath, aScore );
            }
            else if ( ext.ToLower().Equals( ".mid" ) )
            {
                ScoreIO.SaveMidiFile( aFilePath, aScore );
            }
            else if ( ext.ToLower().Equals( ".tech" ) )
            {
                ScoreIO.SaveTechManiaFile( aFilePath, aScore );
            }
            else if ( ext.ToLower().Equals( ".mp4" ) )
            {
                // TODO: MP4の出力処理改善が必要

                using var size = DMS.PlayerForm?.GetFrame( 0 );

                if ( size == null )
                {
                    Log.Error( $"frame read failure.", true );
                    return false;
                }

                var mp4_codec = Config.Media.OutputVideoCodec;
                var fps       = Config.Media.OutputVideoFps;

                try
                {
                    using var mp4 = new Mp4IO();

                    var bmp = mp4.Open( aFilePath, mp4_codec, fps, size.Width, size.Height );

                    if ( bmp == null )
                    {
                        Log.Error( $"open video file failure.", true );
                        return false;
                    }

                    DmsControl.RecordPreSequence();
                    //DMS.PlayerForm.Visible = true;

                    var frameTime = 1D / fps;

                    DmsControl.WaitRecorder();

                    int log_cnt = 0;

                    for ( var time = 0D; time <= DmsControl.EndPlayTime; time += frameTime )
                    {
                        if ( log_cnt++ % fps == 0 )
                        { 
                            Log.Info( $"{(int)time}/{(int)DmsControl.EndPlayTime}({Math.Round( time*100/DmsControl.EndPlayTime, 2 )}%)", true );
                        }

                        using var frame = DMS.PlayerForm?.GetFrame( time );

                        if ( frame == null )
                        {
                            Log.Error( $"frame read error.[{time}]" );
                            continue;
                        }

                        var frameData = frame.LockBits
				                (
					                new( 0, 0, frame.Width, frame.Height ),
					                ImageLockMode.ReadOnly,
                                    frame.PixelFormat
				                );

                        var bmpData = bmp.LockBits
				                (
					                new( 0, 0, bmp.Width, bmp.Height ),
					                ImageLockMode.WriteOnly,
					                bmp.PixelFormat
				                );

                        var buffer = new byte[ frameData.Stride * frameData.Height ];

                        Marshal.Copy( frameData.Scan0, buffer, 0, buffer.Length );
                        Marshal.Copy( buffer, 0, bmpData.Scan0, buffer.Length );

                        frame.UnlockBits( frameData );
			            bmp.UnlockBits( bmpData );

                        mp4.AddFrame();
                    }
                }
                catch ( Exception e )
                {
                    Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
                    return false;
                }
                finally
                {
                    DmsControl.StopPreSequence();

                    //DMS.PlayerForm.Visible = false;
                }
            }
            else
            {
                throw new NotSupportedException( $"Extension {ext} is not supported" );
            }

            Log.Info( $"Succeeded in writing [{aFilePath.AbsoulteFilePath}]", true );
        }
        catch ( Exception e )
        {
            Log.Error( $"Failed to write [{aFilePath.AbsoulteFilePath}]" );
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
            return false;
        }
        return true;
    }

    /// <summary>
    /// MidiMapセットテンプレートの読込
    /// </summary>
    /// <param name="aFilePath">テンプレートファイルパス</param>
    /// <param name="aMidiMapSet">出力MidiMapセット</param>
    /// <returns>True:読込成功、False:読込失敗</returns>
    public static bool LoadMidiMapSet( GeneralPath aFilePath, out MidiMapSet aMidiMapSet )
    {
		using var _ = new LogBlock( Log.GetThisMethodName );

        try
        {
            ScoreIO.LoadFile( aFilePath, out aMidiMapSet );

            Log.Info( $"Succeeded in reading [{aFilePath.AbsoulteFilePath}]" );
        }
        catch ( Exception e )
        {
            Log.Error( $"Failed to read [{aFilePath.AbsoulteFilePath}]" );
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );

            aMidiMapSet = new();
            return false;
        }
        return true;
    }

    /// <summary>
    /// MidiMapセットテンプレートの保存
    /// </summary>
    /// <param name="aFilePath">テンプレートファイルパス</param>
    /// <param name="aMidiMapSet">保存MidiMapセット</param>
    /// <returns>True:保存成功、False:保存失敗</returns>
    public static bool SaveMidiMapSet( GeneralPath aFilePath, MidiMapSet aMidiMapSet )
    {
		using var _ = new LogBlock( Log.GetThisMethodName );

        try
        {
            ScoreIO.SaveFile( aFilePath, aMidiMapSet );

            Log.Info( $"Succeeded in writing [{aFilePath.AbsoulteFilePath}]" );
        }
        catch ( Exception e )
        {
            Log.Error( $"Failed to write [{aFilePath.AbsoulteFilePath}]" );
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
            return false;
        }
        return true;
    }

    /// <summary>
    /// スコア情報のインポート
    /// </summary>
    /// <param name="aFilePath">スコアファイルパス</param>
    /// <param name="aScore">出力先スコア</param>
    /// <returns>True:インポート成功、False:インポート失敗</returns>
    public static bool ImportScore( GeneralPath aFilePath, ref Score aScore )
    {
        // TODO: この辺は改良の余地あり。

		using var _ = new LogBlock( Log.GetThisMethodName );

        try
        {
            var ext = aFilePath.Extension.ToLower();

            //if ( ext.Equals( ".mid" ) )
            //{
            //    ScoreIO.LoadMidiFile( aFilePath, out aScore );
            //}
            //else
            {
                throw new NotSupportedException( $"Extension {ext} is not supported" );
            }

            //Log.Info( $"Succeeded in reading [{aFilePath.AbsoulteFilePath}]" );
        }
        catch ( Exception e )
        {
            Log.Error( $"Failed to read [{aFilePath.AbsoulteFilePath}]" );
            Log.Error( $"{Log.GetThisMethodName}:{e.Message}" );
            return false;
        }
        //return true;
    }
}
