// Type: ZuneUI.TransportControls
// Assembly: ZuneShell, Version=4.7.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: FC8028F3-A47B-4FB4-B35B-11D1752D8264
// Assembly location: C:\Program Files\Zune\ZuneShell.dll

using Microsoft.Iris;
using Microsoft.Zune.Configuration;
using Microsoft.Zune.ErrorMapperApi;
using Microsoft.Zune.PerfTrace;
using Microsoft.Zune.QuickMix;
using Microsoft.Zune.Service;
using Microsoft.Zune.Util;
using MicrosoftZuneLibrary;
using MicrosoftZunePlayback;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UIXControls;

namespace ZuneUI
{
  public class TransportControls : SingletonModelItem<TransportControls>
  {
    private static string _savedNowPlayingFilename = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Shell.LoadString(StringId.IDS_APPDATAFOLDERNAME).TrimStart(new char[1]
    {
      '\\'
    })), "NowPlaying.dat");
    private List<PlaybackTrack> _tracksSubmittedToPlayer = new List<PlaybackTrack>(2);
    private Dictionary<PlaybackTrack, int> _errors = new Dictionary<PlaybackTrack, int>();
    private bool _showErrors = true;
    private bool _playlistSupportsShuffle = true;
    private int _currentTrackIndex = -1;
    private DateTime _currentPlayStartTime = DateTime.MinValue;
    internal const long c_TicksPerSecond = 10000000L;
    private const long _rewindDelay = 50000000L;
    private const float c_overscanFactor = 0.1f;
    private const int c_maxConsecutiveErrors = 5;
    private const int c_ratingUnrated = -1;
    private const string c_knownInvalidUri = ".:* INVALID URI *:.";
    private PlayerInterop _playbackWrapper;
    private BooleanChoice _shuffling;
    private BooleanChoice _repeating;
    private BooleanChoice _muted;
    private BooleanChoice _showTotalTime;
    private BooleanChoice _showNowPlayingList;
    private BooleanChoice _fastforwarding;
    private BooleanChoice _rewinding;
    private RangedValue _volume;
    private Command _play;
    private Command _pause;
    private Command _back;
    private Command _forward;
    private Command _stop;
    private bool _playingVideo;
    private bool _opening;
    private bool _buffering;
    private bool _seekEnabled;
    private int _zoomScaleFactor;
    private Command _fastforwardhotkey;
    private Command _rewindhotkey;
    private NowPlayingList _playlistPending;
    private NowPlayingList _playlistCurrent;
    private Microsoft.Iris.Timer _timerDelayedConfigPersist;
    private VideoStream _videoStream;
    private MCPlayerState _lastKnownPlayerState;
    private MCTransportState _lastKnownTransportState;
    private long _lastKnownPosition;
    private Notification _nowPlayingNotification;
    private float _currentTrackDuration;
    private float _currentTrackPosition;
    private float _downloadProgress;
    private PlaybackTrack _lastKnownPreparedTrack;
    private PlaybackTrack _lastKnownPlaybackTrack;
    private int _consecutiveErrors;
    private int _lastKnownSetUriCallID;
    private Shell _shellInstance;
    private TransportControls.PlayerState _playerState;
    private bool _streamingReportIsOpen;
    private Guid _streamingReportMediaId;
    private Guid _streamingReportMediaInstanceId;
    private TaskbarPlayer _taskbarPlayer;
    private bool _isInitialized;
    private bool _resumeLastNowPlayingRequested;
    private bool _shuffleAllRequested;
    private JumpListPin _requestedJumpListPin;
    private PlaybackContext _pagePlaybackContext;
    private PlaybackTrack _currentTrack;
    private bool _isPlaying;
    private bool _hasPlaylist;
    private bool _hasPlayed;
    private ArrayListDataSet _currentPlaylist;
    private bool _isContextCompatible;
    private bool _isStreamingVideo;
    private bool _supressDownloads;
    private List<TransportControls.SpectrumOutputConfig> _spectrumConfigList;
    private bool _isSpectrumAvailable;
    private int _lastKnownCurrentTrackRating;
    private EventHandler _currentTrackRatingChangedEventHandler;
    private Microsoft.Iris.Timer _isStreamingTimeoutTimer;
    private bool _dontPlayMarketplaceTracks;
    private int _bandwidthCapacity;
    private BandwidthUpdateArgs _bandwidthUpdateInfo;

    public bool IsInitialized
    {
      get
      {
        return this._isInitialized;
      }
      private set
      {
        if (this._isInitialized == value)
          return;
        this._isInitialized = value;
        this.FirePropertyChanged("IsInitialized");
      }
    }

    public bool HasPlayed
    {
      get
      {
        return this._hasPlayed;
      }
      private set
      {
        if (this._hasPlayed == value)
          return;
        this._hasPlayed = value;
        this.FirePropertyChanged("HasPlayed");
      }
    }

    public BooleanChoice Shuffling
    {
      get
      {
        return this._shuffling;
      }
    }

    public BooleanChoice Repeating
    {
      get
      {
        return this._repeating;
      }
    }

    public BooleanChoice Muted
    {
      get
      {
        return this._muted;
      }
    }

    public BooleanChoice ShowTotalTime
    {
      get
      {
        return this._showTotalTime;
      }
    }

    public BooleanChoice ShowNowPlayingList
    {
      get
      {
        return this._showNowPlayingList;
      }
    }

    public BooleanChoice Fastforwarding
    {
      get
      {
        return this._fastforwarding;
      }
    }

    public BooleanChoice Rewinding
    {
      get
      {
        return this._rewinding;
      }
    }

    public RangedValue Volume
    {
      get
      {
        return this._volume;
      }
    }

    public Command Play
    {
      get
      {
        return this._play;
      }
    }

    public Command Pause
    {
      get
      {
        return this._pause;
      }
    }

    public Command Stop
    {
      get
      {
        return this._stop;
      }
    }

    public Command Back
    {
      get
      {
        return this._back;
      }
    }

    public Command Forward
    {
      get
      {
        return this._forward;
      }
    }

    public bool Opening
    {
      get
      {
        return this._opening;
      }
      private set
      {
        if (this._opening == value)
          return;
        this._opening = value;
        this.FirePropertyChanged("Opening");
      }
    }

    public bool Buffering
    {
      get
      {
        return this._buffering;
      }
      private set
      {
        if (this._buffering == value)
          return;
        this._buffering = value;
        this.FirePropertyChanged("Buffering");
      }
    }

    public bool IsSeekEnabled
    {
      get
      {
        return this._seekEnabled;
      }
      private set
      {
        if (this._seekEnabled == value)
          return;
        this._seekEnabled = value;
        this.FirePropertyChanged("IsSeekEnabled");
      }
    }

    public bool IsStreamingVideo
    {
      get
      {
        return this._isStreamingVideo;
      }
      private set
      {
        if (this._isStreamingVideo == value)
          return;
        this._isStreamingVideo = value;
        this.FirePropertyChanged("IsStreamingVideo");
      }
    }

    public bool SupressDownloads
    {
      get
      {
        return this._supressDownloads;
      }
      private set
      {
        if (this._supressDownloads == value)
          return;
        this._supressDownloads = value;
        this.FirePropertyChanged("SupressDownloads");
      }
    }

    public int ZoomScaleFactor
    {
      get
      {
        return this._zoomScaleFactor;
      }
      set
      {
        this._zoomScaleFactor = value;
        this.FirePropertyChanged("ZoomScaleFactor");
      }
    }

    public bool Playing
    {
      get
      {
        return this._isPlaying;
      }
    }

    public bool PlayingVideo
    {
      get
      {
        return this._playingVideo;
      }
      private set
      {
        if (this._playingVideo == value)
          return;
        this._playingVideo = value;
        this.FirePropertyChanged("PlayingVideo");
      }
    }

    public Command FastforwardHotkey
    {
      get
      {
        return this._fastforwardhotkey;
      }
    }

    public Command RewindHotkey
    {
      get
      {
        return this._rewindhotkey;
      }
    }

    public int CurrentTrackIndex
    {
      get
      {
        return this._currentTrackIndex;
      }
    }

    public float CurrentTrackDuration
    {
      get
      {
        return this._currentTrackDuration;
      }
      private set
      {
        if ((double) this._currentTrackDuration == (double) value)
          return;
        this._currentTrackDuration = value;
        this.FirePropertyChanged("CurrentTrackDuration");
      }
    }

    public float CurrentTrackPosition
    {
      get
      {
        return this._currentTrackPosition;
      }
      private set
      {
        if ((double) this._currentTrackPosition == (double) value)
          return;
        this._currentTrackPosition = value;
        this.FirePropertyChanged("CurrentTrackPosition");
      }
    }

    public float CurrentTrackDownloadProgress
    {
      get
      {
        return this._downloadProgress;
      }
      private set
      {
        if ((double) this._downloadProgress == (double) value)
          return;
        this._downloadProgress = value;
        this.FirePropertyChanged("CurrentTrackDownloadProgress");
      }
    }

    public PlaybackTrack CurrentTrack
    {
      get
      {
        return this._currentTrack;
      }
    }

    public int CurrentTrackRating
    {
      get
      {
        if (this.CurrentTrack == null || !this.CurrentTrack.CanRate)
          return 0;
        else
          return this.CurrentTrack.UserRating;
      }
    }

    public bool ShowErrors
    {
      get
      {
        return this._showErrors;
      }
      set
      {
        if (this._showErrors == value)
          return;
        this._showErrors = value;
        this.FirePropertyChanged("ShowErrors");
      }
    }

    public int ErrorCount
    {
      get
      {
        return this._errors.Count;
      }
    }

    public VideoStream VideoStream
    {
      get
      {
        return this._videoStream;
      }
    }

    public bool CanRender3DVideo
    {
      get
      {
        return Application.RenderingType != RenderingType.GDI;
      }
    }

    public bool HasPlaylist
    {
      get
      {
        return this._hasPlaylist;
      }
    }

    public bool PlaylistSupportsShuffle
    {
      get
      {
        return this._playlistSupportsShuffle;
      }
    }

    public ArrayListDataSet CurrentPlaylist
    {
      get
      {
        return this._currentPlaylist;
      }
    }

    public string QuickMixTitle
    {
      get
      {
        string str = string.Empty;
        if (this._playlistCurrent != null)
          str = this._playlistCurrent.QuickMixTitle;
        return str;
      }
    }

    public EQuickMixType QuickMixType
    {
      get
      {
        EQuickMixType equickMixType = EQuickMixType.eQuickMixTypeInvalid;
        if (this._playlistCurrent != null)
          equickMixType = this._playlistCurrent.QuickMixType;
        return equickMixType;
      }
    }

    public bool DontPlayMarketplaceTracks
    {
      get
      {
        return this._dontPlayMarketplaceTracks;
      }
      set
      {
        if (value == this._dontPlayMarketplaceTracks)
          return;
        this._dontPlayMarketplaceTracks = value;
        if (this._playlistCurrent != null)
          this._playlistCurrent.DontPlayMarketplaceTracks = this._dontPlayMarketplaceTracks;
        if (this._playlistPending == null)
          return;
        this._playlistPending.DontPlayMarketplaceTracks = this._dontPlayMarketplaceTracks;
      }
    }

    public QuickMixSession QuickMixSession
    {
      get
      {
        QuickMixSession quickMixSession = (QuickMixSession) null;
        if (this._playlistCurrent != null)
          quickMixSession = this._playlistCurrent.QuickMixSession;
        return quickMixSession;
      }
    }

    public bool IsPlaybackContextCompatible
    {
      get
      {
        return this._isContextCompatible;
      }
    }

    public JumpListPin RequestedJumpListPin
    {
      get
      {
        if (!this.IsInitialized)
          return (JumpListPin) null;
        else
          return this._requestedJumpListPin;
      }
      set
      {
        if (this._requestedJumpListPin == value)
          return;
        this._requestedJumpListPin = value;
        if (!this.IsInitialized)
          return;
        this.FirePropertyChanged("RequestedJumpListPin");
      }
    }

    public bool ShuffleAllRequested
    {
      get
      {
        if (!this.IsInitialized)
          return false;
        else
          return this._shuffleAllRequested;
      }
      set
      {
        if (this._shuffleAllRequested == value)
          return;
        this._shuffleAllRequested = value;
        if (!this.IsInitialized)
          return;
        this.FirePropertyChanged("ShuffleAllRequested");
      }
    }

    public int BandwidthCapacity
    {
      get
      {
        return this._bandwidthCapacity;
      }
      private set
      {
        this._bandwidthCapacity = value;
        this.FirePropertyChanged("BandwidthCapacity");
      }
    }

    public BandwidthUpdateArgs BandwidthUpdateInfo
    {
      get
      {
        return this._bandwidthUpdateInfo;
      }
      private set
      {
        this._bandwidthUpdateInfo = value;
        this.FirePropertyChanged("BandwidthUpdateInfo");
      }
    }

    public event EventHandler PlaybackStopped;

    static TransportControls()
    {
    }

    public TransportControls()
    {
      this._playbackWrapper = PlayerInterop.Instance;
      this._taskbarPlayer = TaskbarPlayer.Instance;
      this._videoStream = new VideoStream();
      if (!this.CanRender3DVideo)
        this._videoStream.DisplayDetailsChanged += new EventHandler(this.OnVideoDetailsChanged);
      this._shuffling = new BooleanChoice((IModelItemOwner) this);
      this._shuffling.Value = ClientConfiguration.Playback.ModeShuffle;
      this._shuffling.ChosenChanged += new EventHandler(this.OnShufflingChanged);
      this.UpdateShufflingDescription();
      this._repeating = new BooleanChoice((IModelItemOwner) this);
      this._repeating.Value = ClientConfiguration.Playback.ModeLoop;
      this._repeating.ChosenChanged += new EventHandler(this.OnRepeatingChanged);
      this.UpdateRepeatingDescription();
      this._muted = new BooleanChoice((IModelItemOwner) this);
      this._muted.Value = ClientConfiguration.Playback.Mute;
      this._muted.ChosenChanged += new EventHandler(this.OnMutingChanged);
      this.UpdateMutingDescription();
      this._showTotalTime = new BooleanChoice((IModelItemOwner) this);
      this._showTotalTime.Value = ClientConfiguration.Playback.ShowTotalTime;
      this._showTotalTime.ChosenChanged += new EventHandler(this.OnShowTotalTimeChanged);
      this._showNowPlayingList = new BooleanChoice((IModelItemOwner) this);
      this._showNowPlayingList.Value = ClientConfiguration.Playback.ShowNowPlayingList;
      this._showNowPlayingList.ChosenChanged += new EventHandler(this.OnShowNowPlayingListChanged);
      this.UpdateShowNowPlayingListDescription();
      this._fastforwarding = new BooleanChoice((IModelItemOwner) this);
      this._fastforwarding.ChosenChanged += new EventHandler(this.OnFastforwardingChanged);
      this._rewinding = new BooleanChoice((IModelItemOwner) this);
      this._rewinding.ChosenChanged += new EventHandler(this.OnRewindingChanged);
      float num = (float) ClientConfiguration.Playback.Volume;
      if ((double) num < 0.0 || (double) num > 100.0)
        num = 50f;
      this._volume = new RangedValue((IModelItemOwner) this);
      this._volume.MinValue = 0.0f;
      this._volume.MaxValue = 100f;
      this._volume.Value = num;
      this._volume.PropertyChanged += new PropertyChangedEventHandler(this.OnVolumeControlChanged);
      this._play = new Command((IModelItemOwner) this, Shell.LoadString(StringId.IDS_PLAY), new EventHandler(this.OnPlayClicked));
      this._play.Available = false;
      this._pause = new Command((IModelItemOwner) this, Shell.LoadString(StringId.IDS_PAUSE), new EventHandler(this.OnPauseClicked));
      this._pause.Available = false;
      this._back = new Command((IModelItemOwner) this, Shell.LoadString(StringId.IDS_PREVIOUS), new EventHandler(this.OnBackClicked));
      this._back.Available = false;
      this._forward = new Command((IModelItemOwner) this, Shell.LoadString(StringId.IDS_NEXT), new EventHandler(this.OnForwardClicked));
      this._forward.Available = false;
      this._stop = new Command((IModelItemOwner) this, Shell.LoadString(StringId.IDS_STOP), new EventHandler(this.OnStopClicked));
      this._stop.Available = false;
      this._fastforwardhotkey = new Command((IModelItemOwner) this, new EventHandler(this.OnFastforwardHotkeyPressed));
      this._rewindhotkey = new Command((IModelItemOwner) this, new EventHandler(this.OnRewindHotkeyPressed));
      this._playbackWrapper.StatusChanged += new EventHandler(this.OnPlaybackStatusChanged);
      this._playbackWrapper.TransportStatusChanged += new EventHandler(this.OnTransportStatusChanged);
      this._playbackWrapper.TransportPositionChanged += new EventHandler(this.OnTransportPositionChanged);
      this._playbackWrapper.UriSet += new EventHandler(this.OnUriSet);
      this._playbackWrapper.AlertSent += new AnnouncementHandler(this.OnAlertSent);
      this._playbackWrapper.PlayerPropertyChanged += new PlayerPropertyChangedEventHandler(this.OnPlayerPropertyChanged);
      this._playbackWrapper.PlayerBandwithUpdate += new PlayerBandwithUpdateEventHandler(this.OnBandwidthCapacityUpdate);
      this._lastKnownPlayerState = this._playbackWrapper.State;
      this._lastKnownTransportState = this._playbackWrapper.TransportState;
      this._lastKnownPosition = this._playbackWrapper.Position;
      this._shellInstance = (Shell) ZuneShell.DefaultInstance;
      this._shellInstance.PropertyChanged += new PropertyChangedEventHandler(this.OnShellPropertyChanged);
      this._timerDelayedConfigPersist = new Microsoft.Iris.Timer();
      this._timerDelayedConfigPersist.Interval = 500;
      this._timerDelayedConfigPersist.AutoRepeat = false;
      this._timerDelayedConfigPersist.Tick += new EventHandler(this.OnDelayedConfigPersistTimerTick);
      this._playerState = TransportControls.PlayerState.Stopped;
      this._spectrumConfigList = new List<TransportControls.SpectrumOutputConfig>();
      this._isSpectrumAvailable = false;
      this.IsSeekEnabled = this._playbackWrapper.CanSeek;
      Download.Instance.DownloadProgressEvent += new DownloadEventProgressHandler(this.OnDownloadProgressed);
      this._lastKnownCurrentTrackRating = -1;
      this._currentTrackRatingChangedEventHandler = new EventHandler(this.OnCurrentTrackRatingChanged);
      this._isStreamingTimeoutTimer = new Microsoft.Iris.Timer((IModelItemOwner) this);
      this._isStreamingTimeoutTimer.Interval = 30000;
      this._isStreamingTimeoutTimer.AutoRepeat = false;
      this._isStreamingTimeoutTimer.Tick += new EventHandler(this.OnIsStreamingTimeout);
      SignIn.Instance.SignInStatusUpdatedEvent += new EventHandler(this.OnSignInEvent);
    }

    protected override void OnDispose(bool fDisposing)
    {
      base.OnDispose(fDisposing);
      if (!fDisposing)
        return;
      this.DisconnectAllSpectrumAnimationSources();
      this._isSpectrumAvailable = false;
      Microsoft.Zune.Util.Notification.ResetNowPlaying();
      if (this.WillSaveCurrentPlaylistOnShutdown())
      {
        try
        {
          using (Stream serializationStream = (Stream) File.Create(TransportControls._savedNowPlayingFilename))
            new BinaryFormatter().Serialize(serializationStream, (object) this._playlistCurrent);
        }
        catch (Exception ex)
        {
        }
      }
      else if (File.Exists(TransportControls._savedNowPlayingFilename))
      {
        try
        {
          File.Delete(TransportControls._savedNowPlayingFilename);
        }
        catch (Exception ex)
        {
        }
      }
      if (this._lastKnownPlaybackTrack != null)
        this._lastKnownPlaybackTrack.OnEndPlayback(false);
      this._playbackWrapper.StatusChanged -= new EventHandler(this.OnPlaybackStatusChanged);
      this._playbackWrapper.TransportStatusChanged -= new EventHandler(this.OnTransportStatusChanged);
      this._playbackWrapper.TransportPositionChanged -= new EventHandler(this.OnTransportPositionChanged);
      this._playbackWrapper.UriSet -= new EventHandler(this.OnUriSet);
      this._playbackWrapper.AlertSent -= new AnnouncementHandler(this.OnAlertSent);
      this._playbackWrapper.PlayerPropertyChanged -= new PlayerPropertyChangedEventHandler(this.OnPlayerPropertyChanged);
      if (this._videoStream != null && !this.CanRender3DVideo)
        this._videoStream.DisplayDetailsChanged -= new EventHandler(this.OnVideoDetailsChanged);
      PlayerInterop.Instance.Dispose();
      if (this._videoStream != null)
      {
        this._videoStream.Dispose();
        this._videoStream = (VideoStream) null;
      }
      if (this._timerDelayedConfigPersist != null)
      {
        this._timerDelayedConfigPersist.Tick -= new EventHandler(this.OnDelayedConfigPersistTimerTick);
        this._timerDelayedConfigPersist.Dispose();
        this._timerDelayedConfigPersist = (Microsoft.Iris.Timer) null;
      }
      this._shellInstance.PropertyChanged -= new PropertyChangedEventHandler(this.OnShellPropertyChanged);
      Download.Instance.DownloadProgressEvent -= new DownloadEventProgressHandler(this.OnDownloadProgressed);
      if (this._currentTrack != null)
        this._currentTrack.RatingChanged.Invoked -= this._currentTrackRatingChangedEventHandler;
      SignIn.Instance.SignInStatusUpdatedEvent -= new EventHandler(this.OnSignInEvent);
    }

    private void OnSignInEvent(object sender, EventArgs args)
    {
      if (this._playlistCurrent != null)
        this._playlistCurrent.UpdateTracks();
      if (this._playlistPending == null)
        return;
      this._playlistPending.UpdateTracks();
    }

    private void PersistSettings()
    {
      if (this._timerDelayedConfigPersist == null)
        return;
      this._timerDelayedConfigPersist.Stop();
      this._timerDelayedConfigPersist.Start();
    }

    private void OnDelayedConfigPersistTimerTick(object sender, EventArgs args)
    {
      ClientConfiguration.Playback.ModeShuffle = this._shuffling.Value;
      ClientConfiguration.Playback.ModeLoop = this._repeating.Value;
      ClientConfiguration.Playback.Mute = this._muted.Value;
      ClientConfiguration.Playback.Volume = (int) this._volume.Value;
      ClientConfiguration.Playback.ShowTotalTime = this._showTotalTime.Value;
      ClientConfiguration.Playback.ShowNowPlayingList = this._showNowPlayingList.Value;
    }

    private void OnCurrentTrackRatingChanged(object sender, EventArgs args)
    {
      int num = -1;
      if (this.CurrentTrack != null && this.CurrentTrack.CanRate)
        num = this.CurrentTrackRating;
      if (num == this._lastKnownCurrentTrackRating)
        return;
      this.FirePropertyChanged("CurrentTrackRating");
      this._lastKnownCurrentTrackRating = num;
    }

    public void TrackRatingUpdatedExternally(int mediaID, int newRating)
    {
      if (this.CurrentPlaylist == null)
        return;
      foreach (PlaybackTrack playbackTrack in (ListDataSet) this.CurrentPlaylist)
      {
        LibraryPlaybackTrack libraryPlaybackTrack = playbackTrack as LibraryPlaybackTrack;
        if (libraryPlaybackTrack != null && libraryPlaybackTrack.MediaId == mediaID)
          libraryPlaybackTrack.RatingUpdatedExternally(newRating);
      }
    }

    private void OnIsStreamingTimeout(object sender, EventArgs args)
    {
      this.SupressDownloads = false;
    }

    private void DeserializeNowPlayingList(object arg)
    {
      string path = arg as string;
      object args = (object) null;
      if (path != null)
      {
        if (File.Exists(path))
        {
          try
          {
            using (Stream serializationStream = (Stream) File.OpenRead(path))
              args = new BinaryFormatter().Deserialize(serializationStream);
          }
          catch (Exception ex)
          {
          }
        }
      }
      if (args == null)
        return;
      Application.DeferredInvoke(new DeferredInvokeHandler(this.DeserializationComplete), args);
    }

    private void DeserializationComplete(object arg)
    {
      NowPlayingList nowPlayingList = arg as NowPlayingList;
      if (this._playlistCurrent != null || nowPlayingList == null)
        return;
      this._playlistCurrent = nowPlayingList;
      this.ShowNotification();
      this.UpdatePropertiesAndCommands();
      if (!this._resumeLastNowPlayingRequested)
        return;
      this.Play.Invoke();
    }

    public bool WillSaveCurrentPlaylistOnShutdown()
    {
      if (this._playlistCurrent != null && this._playlistCurrent.CurrentTrack != null && !this._playlistCurrent.CurrentTrack.IsVideo)
        return this._playlistCurrent.QuickMixSession == null;
      else
        return false;
    }

    public void StartPlayingAt(PlaybackTrack track)
    {
      if (this._playlistCurrent == null || this._playlistCurrent.TrackList == null)
        return;
      int newCurrentIndex = this._playlistCurrent.TrackList.IndexOf((object) track);
      if (newCurrentIndex <= -1)
        return;
      this.StartPlayingAt(newCurrentIndex);
    }

    public void StartPlayingAt(int newCurrentIndex)
    {
      if (this._playlistCurrent == null)
        return;
      this._playlistCurrent.MoveToTrackIndex(newCurrentIndex);
      if (this._playerState == TransportControls.PlayerState.Playing)
      {
        this.SetUriOnPlayer();
      }
      else
      {
        this._playlistPending = this._playlistCurrent;
        if (this._playerState == TransportControls.PlayerState.Paused)
          this._playbackWrapper.Stop();
        else
          this.PlayPendingList();
      }
    }

    public void CloseCurrentSession()
    {
      this.Stop.Invoke();
    }

    public void SeekToPosition(float value)
    {
      long offsetIn100nsUnits = (long) ((double) value * 10000000.0);
      if (this._playbackWrapper != null)
        this._playbackWrapper.SeekToAbsolutePosition(offsetIn100nsUnits);
      this._rewinding.Value = false;
      this._fastforwarding.Value = false;
    }

    public void ClearAllErrors()
    {
      if (this._errors.Count <= 0)
        return;
      this._errors.Clear();
      this.FirePropertyChanged("ErrorCount");
    }

    public bool IsCurrentTrack(Guid zuneMediaId)
    {
      PlaybackTrack currentTrack = this.CurrentTrack;
      if (currentTrack != null && !GuidHelper.IsEmpty(currentTrack.ZuneMediaId))
        return currentTrack.ZuneMediaId == zuneMediaId;
      else
        return false;
    }

    public int GetErrorCode(Guid zuneMediaId)
    {
      if (this._errors.Count > 0)
      {
        foreach (KeyValuePair<PlaybackTrack, int> keyValuePair in this._errors)
        {
          PlaybackTrack key = keyValuePair.Key;
          if (key != null && key.ZuneMediaId == zuneMediaId)
            return keyValuePair.Value;
        }
      }
      return 0;
    }

    public bool IsCurrentTrack(int id, MediaType type, Guid zuneMediaId)
    {
      LibraryPlaybackTrack libraryPlaybackTrack = this.CurrentTrack as LibraryPlaybackTrack;
      if (libraryPlaybackTrack == null)
        return this.IsCurrentTrack(zuneMediaId);
      if (libraryPlaybackTrack.MediaId == id)
        return libraryPlaybackTrack.MediaType == type;
      else
        return false;
    }

    public int GetLibraryErrorCode(int id, MediaType type)
    {
      if (this._errors.Count > 0)
      {
        foreach (KeyValuePair<PlaybackTrack, int> keyValuePair in this._errors)
        {
          LibraryPlaybackTrack libraryPlaybackTrack = keyValuePair.Key as LibraryPlaybackTrack;
          if (libraryPlaybackTrack != null && libraryPlaybackTrack.MediaId == id && libraryPlaybackTrack.MediaType == type)
            return keyValuePair.Value;
        }
      }
      return 0;
    }

    public int GetLibraryErrorCode(PlaybackTrack track)
    {
      int num;
      if (this._errors.TryGetValue(track, out num))
        return num;
      else
        return 0;
    }

    internal void ClearError(PlaybackTrack track)
    {
      if (!this._errors.Remove(track))
        return;
      this.FirePropertyChanged("ErrorCount");
    }

    private void OnBandwidthCapacityUpdate(object sender, BandwidthUpdateArgs args)
    {
      Application.DeferredInvoke(new DeferredInvokeHandler(this.OnBandwidthCapacityUpdateOnApp), (object) args);
    }

    private void OnBandwidthCapacityUpdateOnApp(object obj)
    {
      if (obj == null)
        return;
      BandwidthUpdateArgs bandwidthUpdateArgs = (BandwidthUpdateArgs) obj;
      if (bandwidthUpdateArgs == null || bandwidthUpdateArgs.currentState != MBRHeuristicState.Playback)
        return;
      this.BandwidthCapacity = bandwidthUpdateArgs.RecentAverageBandwidth;
      this.BandwidthUpdateInfo = bandwidthUpdateArgs;
    }

    private void OnStreamingRestrictionResponse(HRESULT hr)
    {
      if (!hr.IsError)
        return;
      string title = Shell.LoadString(StringId.IDS_PLAYBACK_CANNOT_PLAY);
      string description;
      if (hr == HRESULT._ZEST_E_MAX_CONCURRENTSTREAMING_EXCEEDED || hr == HRESULT._ZEST_E_MULTITUNER_CONCURRENTSTREAMING_DETECTED || hr == HRESULT._ZEST_E_MEDIAINSTANCE_STREAMING_OCCUPIED)
        description = ErrorMapperApi.GetMappedErrorDescriptionAndUrl(hr.Int).Description;
      else
        description = Shell.LoadString(StringId.IDS_PLAYBACK_UNKNOWN_CONCURRENT_STREAMING_RESTRICTION);
      Application.DeferredInvoke((DeferredInvokeHandler) delegate
      {
        this.Stop.Invoke();
        MessageBox.Show(title, description, (EventHandler) null);
      }, DeferredInvokePriority.Low);
    }

    private void ReportStreamingAction(TransportControls.PlayerState previousPlayerState)
    {
      if (this.CurrentTrack != null && this.CurrentTrack.IsVideo && this.CurrentTrack.IsStreaming)
      {
        Guid zuneMediaId = this.CurrentTrack.ZuneMediaId;
        if (zuneMediaId != this._streamingReportMediaId && this._streamingReportIsOpen)
        {
          this._streamingReportIsOpen = false;
          previousPlayerState = TransportControls.PlayerState.Stopped;
          Service.Instance.ReportStreamingAction(Microsoft.Zune.Service.EStreamingActionType.Stop, this._streamingReportMediaInstanceId, new AsyncCompleteHandler(this.OnStreamingRestrictionResponse));
        }
        if (previousPlayerState == TransportControls.PlayerState.Stopped && this._playerState == TransportControls.PlayerState.Playing)
        {
          this._streamingReportMediaInstanceId = this.CurrentTrack.ZuneMediaInstanceId;
          if (!(this._streamingReportMediaInstanceId != Guid.Empty))
            return;
          this._streamingReportIsOpen = true;
          this._streamingReportMediaId = zuneMediaId;
          Service.Instance.ReportStreamingAction(Microsoft.Zune.Service.EStreamingActionType.Start, this._streamingReportMediaInstanceId, new AsyncCompleteHandler(this.OnStreamingRestrictionResponse));
        }
        else if (previousPlayerState == TransportControls.PlayerState.Paused && this._playerState == TransportControls.PlayerState.Playing)
          Service.Instance.ReportStreamingAction(Microsoft.Zune.Service.EStreamingActionType.Resume, this._streamingReportMediaInstanceId, new AsyncCompleteHandler(this.OnStreamingRestrictionResponse));
        else if (this._playerState == TransportControls.PlayerState.Paused)
        {
          Service.Instance.ReportStreamingAction(Microsoft.Zune.Service.EStreamingActionType.Pause, this._streamingReportMediaInstanceId, new AsyncCompleteHandler(this.OnStreamingRestrictionResponse));
        }
        else
        {
          if (this._playerState != TransportControls.PlayerState.Stopped)
            return;
          this._streamingReportIsOpen = false;
          Service.Instance.ReportStreamingAction(Microsoft.Zune.Service.EStreamingActionType.Stop, this._streamingReportMediaInstanceId, new AsyncCompleteHandler(this.OnStreamingRestrictionResponse));
        }
      }
      else
      {
        if (!this._streamingReportIsOpen)
          return;
        this._streamingReportIsOpen = false;
        Service.Instance.ReportStreamingAction(Microsoft.Zune.Service.EStreamingActionType.Stop, this._streamingReportMediaInstanceId, new AsyncCompleteHandler(this.OnStreamingRestrictionResponse));
      }
    }

    private void SetPlayerState(TransportControls.PlayerState stateNew)
    {
      TransportControls.PlayerState previousPlayerState = this._playerState;
      if (stateNew != this._playerState)
      {
        this._playerState = stateNew;
        if (this._playerState == TransportControls.PlayerState.Stopped)
        {
          this._rewinding.Value = false;
          this._fastforwarding.Value = false;
          this._lastKnownSetUriCallID = this._lastKnownSetUriCallID + 1;
          this.FirePropertyChanged("PlaybackStopped");
          if (this.PlaybackStopped != null)
            this.PlaybackStopped((object) this, (EventArgs) null);
        }
      }
      this.UpdatePropertiesAndCommands();
      this.ReportStreamingAction(previousPlayerState);
    }

    public void PlayItem(object item, PlayNavigationOptions playNavigationOptions, PlaybackContext playbackContext)
    {
      ArrayListDataSet arrayListDataSet = new ArrayListDataSet();
      arrayListDataSet.Add(item);
      this.PlayItemsWorker((IList) arrayListDataSet, -1, true, playNavigationOptions, playbackContext, (ContainerPlayMarker) null);
    }

    public void PlayItem(object item)
    {
      this.PlayItem(item, PlayNavigationOptions.NavigateVideosToNowPlaying);
    }

    public void PlayItem(object item, PlayNavigationOptions playNavigationOptions)
    {
      ArrayListDataSet arrayListDataSet = new ArrayListDataSet();
      arrayListDataSet.Add(item);
      this.PlayItemsWorker((IList) arrayListDataSet, -1, true, playNavigationOptions, (ContainerPlayMarker) null);
    }

    public void PlayItem(object item, PlaybackContext playbackContext)
    {
      ArrayListDataSet arrayListDataSet = new ArrayListDataSet();
      arrayListDataSet.Add(item);
      this.PlayItemsWorker((IList) arrayListDataSet, -1, true, PlayNavigationOptions.NavigateVideosToNowPlaying, playbackContext, (ContainerPlayMarker) null);
    }

    public void PlayItems(IList items)
    {
      this.PlayItemsWorker(items, -1, true, PlayNavigationOptions.NavigateVideosToNowPlaying, (ContainerPlayMarker) null);
    }

    public void PlayItems(IList items, PlayNavigationOptions playNavigationOptions)
    {
      this.PlayItemsWorker(items, -1, true, playNavigationOptions, (ContainerPlayMarker) null);
    }

    public void PlayItems(IList items, PlayNavigationOptions playNavigationOptions, ContainerPlayMarker containerPlayMarker)
    {
      this.PlayItemsWorker(items, -1, true, playNavigationOptions, containerPlayMarker);
    }

    public void PlayItems(IList items, PlayNavigationOptions playNavigationOptions, PlaybackContext playbackContext)
    {
      this.PlayItemsWorker(items, -1, true, playNavigationOptions, playbackContext, (ContainerPlayMarker) null);
    }

    public void PlayItems(IList items, PlaybackContext playbackContext)
    {
      this.PlayItemsWorker(items, -1, true, PlayNavigationOptions.NavigateVideosToNowPlaying, playbackContext, (ContainerPlayMarker) null);
    }

    public void PlayItems(IList items, int startIndex)
    {
      this.PlayItemsWorker(items, startIndex, true, PlayNavigationOptions.NavigateVideosToNowPlaying, (ContainerPlayMarker) null);
    }

    public void PlayItems(IList items, int startIndex, PlayNavigationOptions playNavigationOptions, ContainerPlayMarker containerPlayMarker)
    {
      this.PlayItemsWorker(items, startIndex, true, PlayNavigationOptions.NavigateVideosToNowPlaying, containerPlayMarker);
    }

    public void AddToNowPlaying(IList items)
    {
      int count = this.PlayItemsWorker(items, -1, false, PlayNavigationOptions.NavigateVideosToNowPlaying, (ContainerPlayMarker) null);
      if (count <= 0)
        return;
      PlaylistManager.Instance.NotifyItemsAdded(-1, count);
    }

    private int PlayItemsWorker(IList items, int startIndex, bool clearQueue, PlayNavigationOptions playNavigationOptions, ContainerPlayMarker containerPlayMarker)
    {
      return this.PlayItemsWorker(items, startIndex, clearQueue, playNavigationOptions, this._shellInstance.CurrentPage.PlaybackContext, containerPlayMarker);
    }

    private int PlayItemsWorker(IList items, int startIndex, bool clearQueue, PlayNavigationOptions playNavigationOptions, PlaybackContext playbackContext, ContainerPlayMarker containerPlayMarker)
    {
      PerfTrace.TraceUICollectionEvent(UICollectionEvent.PlayRequestIssued, "");
      bool flag = this._playlistCurrent != null;
      int num;
      if (clearQueue || !flag)
      {
        if (clearQueue || this._playlistPending == null)
        {
          if (this._playlistPending != null)
            this._playlistPending.Dispose();
          this._playlistPending = new NowPlayingList(items, startIndex, playbackContext, playNavigationOptions, this._shuffling.Value, containerPlayMarker, this._dontPlayMarketplaceTracks);
          num = this._playlistPending.Count;
        }
        else
          num = this._playlistPending.AddItems(items);
        if (playbackContext == PlaybackContext.QuickMix)
        {
          if (this._lastKnownTransportState == MCTransportState.Playing || this._lastKnownTransportState == MCTransportState.Paused)
            this._playbackWrapper.Stop();
          else
            this._playlistPending.PlayWhenReady = true;
        }
        else if (this._playlistPending.Count == 0)
        {
          this._playlistPending.Dispose();
          this._playlistPending = (NowPlayingList) null;
          this.Stop.Invoke();
        }
        else if (flag && (this._lastKnownTransportState == MCTransportState.Playing || this._lastKnownTransportState == MCTransportState.Paused))
          this._playbackWrapper.Stop();
        else
          this.PlayPendingList();
      }
      else
      {
        num = this._playlistCurrent.AddItems(items);
        if (this._playlistPending != null)
          this._playlistPending.Dispose();
        this._playlistPending = (NowPlayingList) null;
        this.UpdateNextTrack();
      }
      return num;
    }

    internal void PlayPendingList()
    {
      if (this._playlistPending == null)
        return;
      if (this._playlistCurrent != null && this._playlistCurrent != this._playlistPending)
        this._playlistCurrent.Dispose();
      this._playlistCurrent = this._playlistPending;
      this._playlistPending = (NowPlayingList) null;
      this._playlistCurrent.SetShuffling(this._shuffling.Value);
      this._playlistCurrent.SetRepeating(this._repeating.Value);
      this._consecutiveErrors = 0;
      if (this._errors.Count > 0)
      {
        this.FirePropertyChanged("ErrorCount");
        this._errors.Clear();
      }
      this.SetPlayerState(TransportControls.PlayerState.Playing);
      this.SetUriOnPlayer();
      bool flag1 = false;
      if (this._playlistCurrent != null)
      {
        PlaybackTrack currentTrack = this._playlistCurrent.CurrentTrack;
        PlayNavigationOptions navigationOptions = this._playlistCurrent.PlayNavigationOptions;
        bool flag2 = false;
        if (currentTrack != null && currentTrack.IsVideo)
          flag1 = true;
        if (navigationOptions == PlayNavigationOptions.NavigateVideosToNowPlaying)
        {
          if (flag1)
            flag2 = true;
        }
        else if (navigationOptions != PlayNavigationOptions.None)
        {
          flag2 = true;
          this._playlistCurrent.PlayNavigationOptions = PlayNavigationOptions.NavigateVideosToNowPlaying;
        }
        if (flag2)
          NowPlayingLand.NavigateToLand(navigationOptions == PlayNavigationOptions.NavigateToNowPlayingWithMix, true);
      }
      this.PlayingVideo = flag1;
    }

    public void RemoveFromNowPlaying(IList indices)
    {
      if (this._playlistCurrent == null)
        return;
      bool flag = this._playlistCurrent.Remove(indices);
      if (this._playlistCurrent.Count == 0)
        this.Stop.Invoke();
      else if (flag)
        this.SetUriOnPlayer();
      else
        this.UpdateNextTrack();
    }

    public void ReorderNowPlaying(IList indices, int targetIndex)
    {
      if (this._playlistCurrent == null)
        return;
      this._playlistCurrent.Reorder(indices, targetIndex);
      this.UpdateNextTrack();
    }

    public IList GetNextTracks(int count)
    {
      IList list = (IList) null;
      if (this._playlistCurrent != null)
        list = this._playlistCurrent.GetNextTracks(count);
      return list;
    }

    public IList CreateAlbumListForBackground(IList allAlbums, int totalDesired)
    {
      List<object> list1 = new List<object>(totalDesired);
      Dictionary<int, object> dictionary = new Dictionary<int, object>();
      if (this._playlistCurrent != null)
      {
        int num = Math.Min(this._playlistCurrent.Count, totalDesired);
        for (int index = 0; index < num; ++index)
        {
          LibraryPlaybackTrack libraryPlaybackTrack = this._playlistCurrent.TrackList[index] as LibraryPlaybackTrack;
          if (libraryPlaybackTrack != null && libraryPlaybackTrack.MediaType == MediaType.Track)
            dictionary[libraryPlaybackTrack.AlbumLibraryId] = (object) null;
        }
      }
      List<object> list2 = new List<object>(totalDesired);
      for (int index = 0; index < allAlbums.Count; ++index)
      {
        DataProviderObject dataProviderObject = (DataProviderObject) allAlbums[index];
        int key = (int) dataProviderObject.GetProperty("LibraryId");
        if ((bool) dataProviderObject.GetProperty("HasAlbumArt"))
        {
          if (dictionary.ContainsKey(key))
            list1.Add((object) dataProviderObject);
          else if (list2.Count < totalDesired)
            list2.Add((object) dataProviderObject);
        }
        if (list1.Count >= totalDesired)
          break;
      }
      for (int index = 0; index < list2.Count && list1.Count < totalDesired; ++index)
        list1.Add(list2[index]);
      if (list1.Count == 0)
        return (IList) null;
      foreach (object album in list1)
        this.DisableSlowDataThumbnailExtraction(album);
      int num1 = 0;
      while (list1.Count < totalDesired)
        list1.Add(list1[num1++]);
      Random random = new Random();
      for (int index1 = list1.Count - 1; index1 > 0; --index1)
      {
        int index2 = random.Next(index1 + 1);
        object obj = list1[index2];
        list1[index2] = list1[index1];
        list1[index1] = obj;
      }
      return (IList) new ListDataSet((IList) list1);
    }

    public void DisableSlowDataThumbnailExtraction(object album)
    {
      LibraryDataProviderItemBase providerItemBase = album as LibraryDataProviderItemBase;
      if (providerItemBase == null)
        return;
      providerItemBase.SetSlowDataThumbnailExtraction(false);
    }

    public void Phase2Init()
    {
      if (!this.CanRender3DVideo)
        this._playbackWrapper.WindowHandle = Application.Window.Handle;
      else
        this._playbackWrapper.DynamicImage = this._videoStream.StreamID;
      ThreadPool.QueueUserWorkItem(new WaitCallback(this.AsyncPhase2Init), (object) null);
    }

    private void AsyncPhase2Init(object arg)
    {
      this._playbackWrapper.Initialize();
      Application.DeferredInvoke(new DeferredInvokeHandler(this.CompletePhase2Init), (object) null);
    }

    private void CompletePhase2Init(object obj)
    {
      this._playbackWrapper.Volume = (int) this._volume.Value;
      this._playbackWrapper.Mute = this._muted.Value;
      this._isSpectrumAvailable = true;
      this.ConnectAllSpectrumAnimationSources();
      this._taskbarPlayer.Initialize(Application.Window.Handle, new TaskbarPlayerCommandHandler(this.OnTaskbarPlayerCommand));
      this.IsInitialized = true;
      ThreadPool.QueueUserWorkItem(new WaitCallback(this.DeserializeNowPlayingList), (object) TransportControls._savedNowPlayingFilename);
      if (this.RequestedJumpListPin != null)
        this.FirePropertyChanged("RequestedJumpListPin");
      if (!this.ShuffleAllRequested)
        return;
      this.FirePropertyChanged("ShuffleAllRequested");
    }

    private void OnTaskbarPlayerCommand(ETaskbarPlayerCommand command, int value)
    {
      switch (command)
      {
        case ETaskbarPlayerCommand.PC_Connect:
          this.UpdateTaskbarPlayer();
          break;
        case ETaskbarPlayerCommand.PC_Play:
          this.Play.Invoke();
          break;
        case ETaskbarPlayerCommand.PC_Pause:
          this.Pause.Invoke();
          break;
        case ETaskbarPlayerCommand.PC_Forward:
          this.Forward.Invoke();
          break;
        case ETaskbarPlayerCommand.PC_Back:
          this.Back.Invoke();
          break;
        case ETaskbarPlayerCommand.PC_Rate:
          if (this.CurrentTrack == null || !this.CurrentTrack.CanRate)
            break;
          switch ((ETaskbarPlayerState) value)
          {
            case ETaskbarPlayerState.PS_RatingNotRated:
              this.CurrentTrack.UserRating = 0;
              break;
            case ETaskbarPlayerState.PS_RatingLoveIt:
              this.CurrentTrack.UserRating = 8;
              break;
            case ETaskbarPlayerState.PS_RatingHateIt:
              this.CurrentTrack.UserRating = 2;
              break;
          }
          this.UpdateTaskbarPlayer();
          break;
      }
    }

    private void UpdateTaskbarPlayer()
    {
      ETaskbarPlayerState etaskbarPlayerState1 = (ETaskbarPlayerState) (0 | (this._playerState == TransportControls.PlayerState.Playing ? 2 : 0) | (this._playerState == TransportControls.PlayerState.Paused ? 4 : 0) | (this._playerState == TransportControls.PlayerState.Stopped ? 1 : 0));
      ETaskbarPlayerState etaskbarPlayerState2;
      if (this.CurrentTrack != null && this.CurrentTrack.CanRate)
      {
        RatingConstants ratingConstants = (RatingConstants) this.CurrentTrack.UserRating;
        etaskbarPlayerState2 = ratingConstants != RatingConstants.Unrated ? (ratingConstants > RatingConstants.MaxHateIt ? etaskbarPlayerState1 | ETaskbarPlayerState.PS_RatingLoveIt : etaskbarPlayerState1 | ETaskbarPlayerState.PS_RatingHateIt) : etaskbarPlayerState1 | ETaskbarPlayerState.PS_RatingNotRated;
      }
      else
        etaskbarPlayerState2 = etaskbarPlayerState1 | ETaskbarPlayerState.PS_RatingNotRated;
      ETaskbarPlayerState state = etaskbarPlayerState2 | (this.Play.Available ? ETaskbarPlayerState.PS_CanPlay : (ETaskbarPlayerState) 0) | (this.Pause.Available ? ETaskbarPlayerState.PS_CanPause : (ETaskbarPlayerState) 0) | (this.Forward.Available ? ETaskbarPlayerState.PS_CanForward : (ETaskbarPlayerState) 0) | (this.Back.Available ? ETaskbarPlayerState.PS_CanBack : (ETaskbarPlayerState) 0);
      if (this.CurrentTrack != null && this.CurrentTrack.CanRate)
        state |= ETaskbarPlayerState.PS_CanRate;
      this._taskbarPlayer.UpdateToolbar(state);
    }

    public int CreateSpectrumAnimationSource(int numBands, bool outputFrequencyData, bool outputWaveformData, bool enableStereoOutput)
    {
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      for (int index = 0; index < numBands; ++index)
      {
        if (outputFrequencyData)
        {
          if (!enableStereoOutput)
          {
            dictionary[string.Format("Frequency{0}", (object) index)] = index;
          }
          else
          {
            dictionary[string.Format("FrequencyL{0}", (object) index)] = index;
            dictionary[string.Format("FrequencyR{0}", (object) index)] = index + 1024;
          }
        }
        if (outputWaveformData)
        {
          if (!enableStereoOutput)
          {
            dictionary[string.Format("Waveform{0}", (object) index)] = index + 2048;
          }
          else
          {
            dictionary[string.Format("WaveformL{0}", (object) index)] = index + 2048;
            dictionary[string.Format("WaveformR{0}", (object) index)] = index + 3072;
          }
        }
      }
      int externalAnimationInput = Application.CreateExternalAnimationInput((IDictionary<string, int>) dictionary);
      TransportControls.SpectrumOutputConfig spectrumOutputConfig = new TransportControls.SpectrumOutputConfig();
      spectrumOutputConfig.SourceId = (uint) externalAnimationInput;
      spectrumOutputConfig.NumBands = (uint) numBands;
      spectrumOutputConfig.Frequency = outputFrequencyData;
      spectrumOutputConfig.Waveform = outputWaveformData;
      spectrumOutputConfig.Stereo = enableStereoOutput;
      spectrumOutputConfig.IsConnected = this._isSpectrumAvailable;
      this._spectrumConfigList.Add(spectrumOutputConfig);
      if (this._isSpectrumAvailable)
        this._playbackWrapper.ConnectAnimationsToSpectrumAnalyzer(spectrumOutputConfig.SourceId, spectrumOutputConfig.NumBands, spectrumOutputConfig.Frequency, spectrumOutputConfig.Waveform, spectrumOutputConfig.Stereo);
      return externalAnimationInput;
    }

    public void DisposeSpectrumAnimationSource(int inputSourceId)
    {
      if (inputSourceId <= 0)
        return;
      for (int index = 0; index < this._spectrumConfigList.Count; ++index)
      {
        TransportControls.SpectrumOutputConfig spectrumOutputConfig = this._spectrumConfigList[index];
        if ((long) spectrumOutputConfig.SourceId == (long) inputSourceId)
        {
          if (spectrumOutputConfig.IsConnected)
          {
            this._playbackWrapper.DisconnectAnimationsFromSpectrumAnalyzer(spectrumOutputConfig.SourceId);
            spectrumOutputConfig.IsConnected = false;
          }
          Application.DisposeExternalAnimationInput(inputSourceId);
          this._spectrumConfigList.RemoveAt(index);
          break;
        }
      }
    }

    public void ConnectAllSpectrumAnimationSources()
    {
      for (int index = 0; index < this._spectrumConfigList.Count; ++index)
      {
        TransportControls.SpectrumOutputConfig spectrumOutputConfig = this._spectrumConfigList[index];
        if (!spectrumOutputConfig.IsConnected)
        {
          this._playbackWrapper.ConnectAnimationsToSpectrumAnalyzer(spectrumOutputConfig.SourceId, spectrumOutputConfig.NumBands, spectrumOutputConfig.Frequency, spectrumOutputConfig.Waveform, spectrumOutputConfig.Stereo);
          spectrumOutputConfig.IsConnected = true;
          this._spectrumConfigList[index] = spectrumOutputConfig;
        }
      }
    }

    public void DisconnectAllSpectrumAnimationSources()
    {
      if (!this._isSpectrumAvailable)
        return;
      for (int index = 0; index < this._spectrumConfigList.Count; ++index)
      {
        TransportControls.SpectrumOutputConfig spectrumOutputConfig = this._spectrumConfigList[index];
        if (spectrumOutputConfig.IsConnected)
        {
          this._playbackWrapper.DisconnectAnimationsFromSpectrumAnalyzer(spectrumOutputConfig.SourceId);
          spectrumOutputConfig.IsConnected = false;
          this._spectrumConfigList[index] = spectrumOutputConfig;
        }
      }
    }

    public void ResumeLastNowPlayingHandler()
    {
      if (!this.IsInitialized)
      {
        this._resumeLastNowPlayingRequested = true;
      }
      else
      {
        if (this._isPlaying)
          return;
        this.Play.Invoke();
      }
    }

    private void OnShellPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      if (!(args.PropertyName == "CurrentPage"))
        return;
      this._pagePlaybackContext = this._shellInstance.CurrentPage.PlaybackContext;
      this.UpdatePropertiesAndCommands();
    }

    private void OnDownloadProgressed(Guid zuneMediaId, float percent)
    {
      if (this.CurrentTrack == null || !(this.CurrentTrack.ZuneMediaId == zuneMediaId))
        return;
      this.CurrentTrackDownloadProgress = percent;
    }

    private void OnShufflingChanged(object sender, EventArgs e)
    {
      if (this._playlistCurrent != null)
      {
        this._playlistCurrent.SetShuffling(this._shuffling.Value);
        this.UpdateNextTrack();
      }
      this.UpdateShufflingDescription();
      this.PersistSettings();
      SQMLog.Log(SQMDataId.ShuffleClicks, 1);
    }

    private void UpdateShufflingDescription()
    {
      this._shuffling.Description = Shell.LoadString(!this._shuffling.Value ? StringId.IDS_SHUFFLE_ON : StringId.IDS_SHUFFLE_OFF);
    }

    private void OnRepeatingChanged(object sender, EventArgs e)
    {
      if (this._playlistCurrent != null)
      {
        this._playlistCurrent.SetRepeating(this._repeating.Value);
        this.UpdateNextTrack();
      }
      SQMLog.Log(SQMDataId.RepeatClicks, 1);
      this.UpdateRepeatingDescription();
      this.PersistSettings();
    }

    private void UpdateRepeatingDescription()
    {
      this._repeating.Description = Shell.LoadString(!this._repeating.Value ? StringId.IDS_REPEAT_ON : StringId.IDS_REPEAT_OFF);
    }

    private void OnMutingChanged(object sender, EventArgs e)
    {
      this.UpdateMutingDescription();
      this._playbackWrapper.Mute = this._muted.Value;
      SQMLog.Log(SQMDataId.VolumeMuteClicks, 1);
      this.PersistSettings();
    }

    private void UpdateMutingDescription()
    {
      this._muted.Description = Shell.LoadString(!this._muted.Value ? StringId.IDS_MUTE : StringId.IDS_UNMUTE);
    }

    private void OnShowTotalTimeChanged(object sender, EventArgs e)
    {
      this.PersistSettings();
    }

    private void OnShowNowPlayingListChanged(object sender, EventArgs e)
    {
      this.UpdateShowNowPlayingListDescription();
      this.PersistSettings();
    }

    private void UpdateShowNowPlayingListDescription()
    {
      this._showNowPlayingList.Description = Shell.LoadString(!this._showNowPlayingList.Value ? StringId.IDS_NOWPLAYINGLIST_ON : StringId.IDS_NOWPLAYINGLIST_OFF);
    }

    private void OnPlayingChanged(object sender, EventArgs e)
    {
    }

    private void OnPlayClicked(object sender, EventArgs e)
    {
      if (!this._play.Available)
        return;
      SQMLog.Log(SQMDataId.PlayClicks, 1);
      if (this.Playing || this._playlistCurrent == null)
        return;
      if (this._lastKnownPlayerState != MCPlayerState.Closed)
      {
        this.SetPlayerState(TransportControls.PlayerState.Playing);
        this._playbackWrapper.Play();
        if (!this.PlayingVideo || this._playlistCurrent.PlayNavigationOptions != PlayNavigationOptions.NavigateVideosToNowPlaying)
          return;
        NowPlayingLand.NavigateToLand();
      }
      else
      {
        if (this._playlistPending != null)
          this._playlistPending.Dispose();
        this._playlistPending = this._playlistCurrent;
        this.PlayPendingList();
      }
    }

    private void OnPauseClicked(object sender, EventArgs e)
    {
      if (!this._pause.Available)
        return;
      SQMLog.Log(SQMDataId.PauseClicks, 1);
      if (!this.Playing)
        return;
      this.SetPlayerState(TransportControls.PlayerState.Paused);
      this._playbackWrapper.Pause();
    }

    private void OnStopClicked(object sender, EventArgs e)
    {
      if (!this._stop.Available)
        return;
      SQMLog.Log(SQMDataId.StopClicks, 1);
      if (this._playlistPending != null)
        this._playlistPending.Dispose();
      this._playlistPending = (NowPlayingList) null;
      if (this._playlistCurrent != null)
        this._playlistCurrent.Dispose();
      this._playlistCurrent = (NowPlayingList) null;
      if (this._playerState != TransportControls.PlayerState.Stopped)
      {
        this.SetPlayerState(TransportControls.PlayerState.Stopped);
        this._playbackWrapper.Stop();
      }
      else
        this.UpdatePropertiesAndCommands();
    }

    private void OnBackClicked(object sender, EventArgs e)
    {
      if (!this._back.Available || this._playlistCurrent == null)
        return;
      SQMLog.Log(SQMDataId.SkipBackwardClicks, 1);
      if (this._lastKnownPosition > 50000000L || !this._playlistCurrent.CanRetreat)
      {
        this._playbackWrapper.SeekToAbsolutePosition(0L);
      }
      else
      {
        this._playlistCurrent.Retreat();
        this.SetUriOnPlayer();
      }
    }

    private void OnForwardClicked(object sender, EventArgs e)
    {
      if (!this._forward.Available || this._playlistCurrent == null)
        return;
      SQMLog.Log(SQMDataId.SkipForwardClicks, 1);
      if (this._currentTrack != null && this._currentTrack.IsVideo)
        return;
      if (this._lastKnownPlaybackTrack != null)
        this._lastKnownPlaybackTrack.OnSkip();
      if (this._playlistCurrent.CanAdvance)
      {
        this._playlistCurrent.Advance();
        this.SetUriOnPlayer();
      }
      else
      {
        this.SetPlayerState(TransportControls.PlayerState.Stopped);
        this._playbackWrapper.Stop();
      }
    }

    private void OnFastforwardingChanged(object sender, EventArgs e)
    {
      if ((this._currentTrack == null || !this._currentTrack.IsVideo || this._playbackWrapper.CanChangeVideoRate) && this._fastforwarding.Value)
      {
        this._rewinding.Value = false;
        this._playbackWrapper.Rate = 5f;
      }
      else
        this._playbackWrapper.Rate = 1f;
    }

    private void OnRewindingChanged(object sender, EventArgs e)
    {
      if ((this._currentTrack == null || !this._currentTrack.IsVideo || this._playbackWrapper.CanChangeVideoRate) && this._rewinding.Value)
      {
        this._fastforwarding.Value = false;
        this._playbackWrapper.Rate = -5f;
      }
      else
        this._playbackWrapper.Rate = 1f;
    }

    private void OnFastforwardHotkeyPressed(object sender, EventArgs e)
    {
      this._fastforwarding.Value = !this._fastforwarding.Value;
    }

    private void OnRewindHotkeyPressed(object sender, EventArgs e)
    {
      this._rewinding.Value = !this._rewinding.Value;
    }

    private void OnPlaybackStatusChanged(object sender, EventArgs e)
    {
      Application.DeferredInvoke(new DeferredInvokeHandler(this.DeferredPlaybackStatusChanged), (object) new object[2]
      {
        (object) this._playbackWrapper.State,
        (object) (bool) (this._playbackWrapper.EndOfMedia ? 1 : 0)
      });
    }

    private void OnTransportStatusChanged(object sender, EventArgs e)
    {
      Application.DeferredInvoke(new DeferredInvokeHandler(this.DeferredTransportStatusChanged), (object) new object[3]
      {
        (object) this._playbackWrapper.TransportState,
        (object) (bool) (this._playbackWrapper.EndOfMedia ? 1 : 0),
        (object) (bool) (this._playbackWrapper.CanSeek ? 1 : 0)
      });
    }

    private void OnTransportPositionChanged(object sender, EventArgs e)
    {
      Application.DeferredInvoke(new DeferredInvokeHandler(this.DeferredTransportPositionChanged), (object) this._playbackWrapper.Position);
    }

    private void OnUriSet(object sender, EventArgs e)
    {
      Application.DeferredInvoke(new DeferredInvokeHandler(this.DeferredUriSet), (object) new object[2]
      {
        (object) this._playbackWrapper.CurrentUri,
        (object) this._playbackWrapper.CurrentUriID
      });
    }

    private void OnAlertSent(Announcement alert)
    {
      Application.DeferredInvoke(new DeferredInvokeHandler(this.DeferredAlertHandler), (object) alert);
    }

    private void OnPlayerPropertyChanged(object sender, PlayerPropertyChangedEventArgs e)
    {
      if (e.Key == "presentationinfo")
        Application.DeferredInvoke(new DeferredInvokeHandler(this.DeferredPresentationInfoChangedHandler), e.Value);
      else if (e.Key == "volumeinfo")
      {
        Application.DeferredInvoke(new DeferredInvokeHandler(this.DeferredVolumeInfoChangedHandler), e.Value);
      }
      else
      {
        if (!(e.Key == "canchangevideorate"))
          return;
        Application.DeferredInvoke(new DeferredInvokeHandler(this.DeferredCanChangeVideoRateHandler), e.Value);
      }
    }

    private void OnVolumeControlChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Value"))
        return;
      this._muted.Value = false;
      this._playbackWrapper.Mute = false;
      this._playbackWrapper.Volume = (int) this._volume.Value;
      SQMLog.Log(SQMDataId.VolumeAdjustmentClicks, 1);
      this.PersistSettings();
    }

    private void OnVideoDetailsChanged(object sender, EventArgs args)
    {
      this._playbackWrapper.VideoPosition = new VideoWindow(this._videoStream.DisplayPosition.X, this._videoStream.DisplayPosition.Y, this._videoStream.DisplayPosition.X + this._videoStream.DisplaySize.Width, this._videoStream.DisplayPosition.Y + this._videoStream.DisplaySize.Height);
      this._playbackWrapper.ShowGDIVideo = this._videoStream.DisplayVisibility;
    }

    private void DeferredPlaybackStatusChanged(object obj)
    {
      if (this.IsDisposed)
        return;
      object[] objArray = (object[]) obj;
      MCPlayerState mcPlayerState = (MCPlayerState) objArray[0];
      bool endOfMedia = (bool) objArray[1];
      bool flag = false;
      if (mcPlayerState == MCPlayerState.Built)
      {
        if (this.Playing && this._lastKnownPlaybackTrack != null)
        {
          this._lastKnownPlaybackTrack.OnEndPlayback(endOfMedia);
          this._lastKnownPlaybackTrack = this._lastKnownPreparedTrack;
          this._lastKnownPreparedTrack = (PlaybackTrack) null;
          if (this._lastKnownPlaybackTrack != null)
            this._lastKnownPlaybackTrack.OnBeginPlayback(this._playbackWrapper);
        }
        this.CurrentTrackDuration = this._playbackWrapper.Duration >= 0L ? (float) this._playbackWrapper.Duration / 1E+07f : 0.0f;
      }
      else if (mcPlayerState == MCPlayerState.Open)
      {
        this._rewinding.Value = false;
        this._fastforwarding.Value = false;
        flag = true;
      }
      else if (mcPlayerState == MCPlayerState.Closed && this.Playing)
        this.SetUriOnPlayer();
      this._lastKnownPlayerState = mcPlayerState;
      if (this._opening == flag)
        return;
      this.Opening = flag;
    }

    private void ShowNotification()
    {
      if (this._nowPlayingNotification == null)
      {
        this._nowPlayingNotification = (Notification) new NowPlayingNotification();
        NotificationArea.Instance.Add(this._nowPlayingNotification);
      }
      else
        NotificationArea.Instance.ForceToFront(this._nowPlayingNotification);
    }

    private void HideNotification()
    {
      if (this._nowPlayingNotification == null)
        return;
      NotificationArea.Instance.Remove(this._nowPlayingNotification);
      this._nowPlayingNotification = (Notification) null;
    }

    public void ShowPreparingNotification()
    {
      this.HideNotification();
      this.HidePreparingNotification(false);
      NotificationArea.Instance.Add((Notification) new PreparingPlayNotification());
    }

    public void HidePreparingNotification()
    {
      this.HidePreparingNotification(true);
    }

    private void HidePreparingNotification(bool restoreNowPlayingNotification)
    {
      NotificationArea.Instance.RemoveAll(NotificationTask.PreparingPlay, NotificationState.Normal);
      if (!restoreNowPlayingNotification || this._playlistCurrent == null)
        return;
      this.ShowNotification();
    }

    private void DeferredTransportStatusChanged(object obj)
    {
      if (this.IsDisposed)
        return;
      object[] objArray = (object[]) obj;
      MCTransportState mcTransportState = (MCTransportState) objArray[0];
      bool endOfMedia = (bool) objArray[1];
      this.IsSeekEnabled = (bool) objArray[2];
      if (mcTransportState == MCTransportState.Buffering)
      {
        this.Buffering = true;
        this.UpdatePropertiesAndCommands();
      }
      else
      {
        if (this.Buffering)
        {
          this.Buffering = false;
          this.UpdatePropertiesAndCommands();
        }
        if (mcTransportState == MCTransportState.Stopped)
        {
          bool flag = this._playlistCurrent != null && this._playlistCurrent.Count == 1 && this._repeating.Value && endOfMedia;
          if (this._lastKnownPlaybackTrack != null)
          {
            this._lastKnownPlaybackTrack.OnEndPlayback(endOfMedia);
            if (!flag)
              this._lastKnownPlaybackTrack = (PlaybackTrack) null;
          }
          if (this._playlistPending != null)
          {
            if (this._playlistPending.TrackList != null && this._playlistPending.TrackList.Count > 0)
            {
              this.PlayPendingList();
            }
            else
            {
              this.SetPlayerState(TransportControls.PlayerState.Stopped);
              this._playlistPending.PlayWhenReady = true;
            }
          }
          else if (this._lastKnownPreparedTrack != null)
          {
            this._lastKnownPlaybackTrack = this._lastKnownPreparedTrack;
            this._lastKnownPlaybackTrack.OnBeginPlayback(this._playbackWrapper);
            if (this.Playing)
              this._playbackWrapper.Play();
            this._lastKnownPreparedTrack = (PlaybackTrack) null;
          }
          else if (flag && this._lastKnownPlaybackTrack != null)
          {
            this._lastKnownPlaybackTrack.OnBeginPlayback(this._playbackWrapper);
            this._playbackWrapper.SeekToAbsolutePosition(0L);
            this._playbackWrapper.Play();
          }
          else
          {
            if (this._playlistCurrent != null)
              this._playlistCurrent.ResetForReplay();
            this.SetPlayerState(TransportControls.PlayerState.Stopped);
            this._playbackWrapper.Close();
          }
        }
        else if (mcTransportState == MCTransportState.Playing)
          PerfTrace.TraceUICollectionEvent(UICollectionEvent.PlayRequestComplete, "");
      }
      this._lastKnownTransportState = mcTransportState;
    }

    private void DeferredTransportPositionChanged(object obj)
    {
      if (this.IsDisposed)
        return;
      this._lastKnownPosition = (long) obj;
      this.CurrentTrackPosition = (float) this._lastKnownPosition / 1E+07f;
      if (this._lastKnownPlaybackTrack == null)
        return;
      if (this._lastKnownPosition > 0L)
        this._consecutiveErrors = 0;
      this._lastKnownPlaybackTrack.OnPositionChanged(this._lastKnownPosition);
    }

    private void DeferredUriSet(object obj)
    {
      if (this.IsDisposed)
        return;
      object[] objArray = (object[]) obj;
      string str = (string) objArray[0];
      int num = (int) objArray[1];
      this._lastKnownPreparedTrack = (PlaybackTrack) null;
      int count;
      for (count = 0; count < this._tracksSubmittedToPlayer.Count; ++count)
      {
        PlaybackTrack playbackTrack = this._tracksSubmittedToPlayer[count];
        if (playbackTrack.PlaybackID == num)
        {
          this._lastKnownPreparedTrack = playbackTrack;
          ++count;
          break;
        }
      }
      this._tracksSubmittedToPlayer.RemoveRange(0, count);
      if (this._lastKnownPreparedTrack == null || this._tracksSubmittedToPlayer.Count != 0)
        return;
      if (this._playlistCurrent != null)
        this._playlistCurrent.SyncCurrentTrackTo(this._lastKnownPreparedTrack);
      this.UpdateNextTrack();
    }

    private void DeferredAlertHandler(object obj)
    {
      if (this.IsDisposed)
        return;
      Announcement announcement = obj as Announcement;
      if (announcement == null)
        return;
      PlaybackTrack track = (PlaybackTrack) null;
      if (this.CurrentTrack != null && this.CurrentTrack.PlaybackID == announcement.PlaybackID)
      {
        track = this.CurrentTrack;
      }
      else
      {
        for (int index = this._tracksSubmittedToPlayer.Count - 1; index >= 0; --index)
        {
          if (this._tracksSubmittedToPlayer[index].PlaybackID == announcement.PlaybackID)
          {
            track = this._tracksSubmittedToPlayer[index];
            break;
          }
        }
      }
      if (track != null)
      {
        ++this._consecutiveErrors;
        this._errors[track] = announcement.HResult;
        if (this._playlistCurrent != null)
          this._playlistCurrent.SyncCurrentTrackTo(track);
        this.FirePropertyChanged("ErrorCount");
      }
      bool flag = (HRESULT) announcement.HResult == HRESULT._NS_E_CD_BUSY;
      if (this._consecutiveErrors < 5 && !flag && this.Playing && (this._playlistCurrent != null && this._playlistCurrent.CanAdvance))
      {
        this._playlistCurrent.Advance();
        this._playbackWrapper.Close();
      }
      else
      {
        this.SetPlayerState(TransportControls.PlayerState.Stopped);
        this._playbackWrapper.Close();
        if (this._lastKnownPlaybackTrack != null)
        {
          this._lastKnownPlaybackTrack.OnEndPlayback(false);
          this._lastKnownPlaybackTrack = (PlaybackTrack) null;
        }
        if (flag || !this.ShowErrors)
          return;
        ErrorDialogInfo.Show(announcement.HResult, Shell.LoadString(StringId.IDS_PLAYBACK_ERROR));
      }
    }

    private void DeferredPresentationInfoChangedHandler(object obj)
    {
      if (this.IsDisposed || this._videoStream == null)
        return;
      PresentationInfo presentationInfo = (PresentationInfo) obj;
      this._videoStream.ContentWidth = presentationInfo.ContentWidth;
      this._videoStream.ContentHeight = presentationInfo.ContentHeight;
      this._videoStream.ContentAspectWidth = presentationInfo.ContentAspectWidth;
      this._videoStream.ContentAspectHeight = presentationInfo.ContentAspectHeight;
      this._videoStream.ContentOverscanPercent = presentationInfo.NeedOverscan ? 0.1f : 0.0f;
      this.FirePropertyChanged("VideoStream");
    }

    private void DeferredVolumeInfoChangedHandler(object obj)
    {
      this._volume.Value = (float) this._playbackWrapper.Volume;
      this.FirePropertyChanged("Volume");
      this._muted.Value = this._playbackWrapper.Mute;
      this.FirePropertyChanged("Mute");
    }

    private void DeferredCanChangeVideoRateHandler(object obj)
    {
      if (this._currentTrack == null || !this._currentTrack.IsVideo)
        return;
      this._forward.Available = this._playbackWrapper.CanChangeVideoRate;
    }

    private void SetUriOnPlayer()
    {
      PlaybackTrack track = (PlaybackTrack) null;
      if (this._playlistCurrent != null)
        track = this._playlistCurrent.CurrentTrack;
      PlaybackTrack nextTrack = (PlaybackTrack) null;
      if (this._playlistCurrent != null && this._playlistCurrent.Count > 1)
        nextTrack = this._playlistCurrent.NextTrack;
      if (track != null)
      {
        this.SetUrisOnPlayerAsync(track, nextTrack);
      }
      else
      {
        this.SetPlayerState(TransportControls.PlayerState.Stopped);
        this._playbackWrapper.Close();
        this.UpdatePropertiesAndCommands();
      }
    }

    private void SetNextUriOnPlayer()
    {
      PlaybackTrack nextTrack = (PlaybackTrack) null;
      if (this._playlistCurrent != null)
        nextTrack = this._playlistCurrent.NextTrack;
      if (nextTrack != null && this._playlistCurrent.Count > 1)
        this.SetUrisOnPlayerAsync((PlaybackTrack) null, nextTrack);
      else
        this._playbackWrapper.CancelNext();
    }

    private void SetUrisOnPlayerAsync(PlaybackTrack track, PlaybackTrack nextTrack)
    {
      int myID = ++this._lastKnownSetUriCallID;
      ThreadPool.QueueUserWorkItem((WaitCallback) (args =>
      {
        if (track != null)
        {
          string trackUri;
          HRESULT local_0 = track.GetURI(out trackUri);
          if (string.IsNullOrEmpty(trackUri))
            trackUri = ".:* INVALID URI *:.";
          if (local_0.IsSuccess)
            Application.DeferredInvoke((DeferredInvokeHandler) delegate
            {
              if (this.IsDisposed || myID != this._lastKnownSetUriCallID)
                return;
              this._playbackWrapper.SetUri(trackUri, 0L, track.PlaybackID);
              this.ReportStreamingAction(TransportControls.PlayerState.Stopped);
              this._tracksSubmittedToPlayer.Remove(track);
              this._tracksSubmittedToPlayer.Add(track);
              if (nextTrack != null)
                return;
              this._playbackWrapper.CancelNext();
              this.UpdatePropertiesAndCommands();
            }, (object) null);
          else
            this.OnAlertSent(new Announcement()
            {
              HResult = local_0.Int,
              PlaybackID = track.PlaybackID
            });
        }
        if (nextTrack == null)
          return;
        string nextTrackUri;
        HRESULT local_4 = nextTrack.GetURI(out nextTrackUri);
        if (string.IsNullOrEmpty(nextTrackUri))
          nextTrackUri = ".:* INVALID URI *:.";
        if (!local_4.IsSuccess)
          return;
        Application.DeferredInvoke((DeferredInvokeHandler) delegate
        {
          if (this.IsDisposed || myID != this._lastKnownSetUriCallID)
            return;
          this._playbackWrapper.SetNextUri(nextTrackUri, 0L, nextTrack.PlaybackID);
          this._tracksSubmittedToPlayer.Remove(nextTrack);
          this._tracksSubmittedToPlayer.Add(nextTrack);
          this.UpdatePropertiesAndCommands();
        }, (object) null);
      }), (object) null);
    }

    private void UpdateNextTrack()
    {
      this.SetNextUriOnPlayer();
      this.UpdatePropertiesAndCommands();
    }

    private bool AreContextsCompatible(PlaybackContext contextCurrent, PlaybackContext contextNext)
    {
      if (contextCurrent == PlaybackContext.None)
        return true;
      else
        return contextNext == contextCurrent;
    }

    public bool IsCurrentPlaylistContextCompatible(PlaybackContext context)
    {
      bool flag = true;
      if (this._playlistCurrent != null)
        flag = this.AreContextsCompatible(context, this._playlistCurrent.PlaybackContext);
      return flag;
    }

    private void UpdatePropertiesAndCommands()
    {
      bool flag1 = this._playerState == TransportControls.PlayerState.Playing;
      ArrayListDataSet arrayListDataSet;
      bool flag2;
      bool flag3;
      PlaybackTrack playbackTrack;
      int num;
      bool flag4;
      if (this._playlistCurrent != null)
      {
        arrayListDataSet = this._playlistCurrent.TrackList;
        flag2 = true;
        flag3 = this.IsCurrentPlaylistContextCompatible(this._pagePlaybackContext);
        playbackTrack = this._playlistCurrent.CurrentTrack;
        num = this._playlistCurrent.ListIndexOfCurrentTrack;
        flag4 = this._playlistCurrent.QuickMixSession == null;
      }
      else
      {
        flag2 = false;
        arrayListDataSet = (ArrayListDataSet) null;
        flag3 = true;
        playbackTrack = (PlaybackTrack) null;
        num = -1;
        this.PlayingVideo = false;
        flag4 = true;
      }
      if (arrayListDataSet != this._currentPlaylist)
      {
        this._currentPlaylist = arrayListDataSet;
        this.FirePropertyChanged("CurrentPlaylist");
      }
      if (flag2 != this._hasPlaylist)
      {
        this._hasPlaylist = flag2;
        this.FirePropertyChanged("HasPlaylist");
      }
      if (flag4 != this._playlistSupportsShuffle)
      {
        this._playlistSupportsShuffle = flag4;
        this.FirePropertyChanged("PlaylistSupportsShuffle");
      }
      if (flag1 != this._isPlaying)
      {
        this._isPlaying = flag1;
        this.FirePropertyChanged("Playing");
        if (this._isPlaying)
        {
          this.HasPlayed = true;
          this._currentPlayStartTime = DateTime.Now;
        }
        else if (this._currentPlayStartTime != DateTime.MinValue)
        {
          Telemetry.Instance.ReportPlaybackTime((int) DateTime.Now.Subtract(this._currentPlayStartTime).TotalSeconds);
          this._currentPlayStartTime = DateTime.MinValue;
        }
      }
      if (num != this._currentTrackIndex)
      {
        if (this._currentPlayStartTime != DateTime.MinValue)
        {
          Telemetry.Instance.ReportPlaybackTime((int) DateTime.Now.Subtract(this._currentPlayStartTime).TotalSeconds);
          this._currentPlayStartTime = DateTime.Now;
        }
        this._currentTrackIndex = num;
        this.FirePropertyChanged("CurrentTrackIndex");
      }
      if (!object.ReferenceEquals((object) playbackTrack, (object) this._currentTrack))
      {
        if (this._currentTrack != null)
          this._currentTrack.RatingChanged.Invoked -= this._currentTrackRatingChangedEventHandler;
        this._currentTrack = playbackTrack;
        if (this._currentTrack != null)
          this._currentTrack.RatingChanged.Invoked += this._currentTrackRatingChangedEventHandler;
        this.FirePropertyChanged("CurrentTrack");
        this.OnCurrentTrackRatingChanged((object) this, (EventArgs) null);
        this.CurrentTrackDownloadProgress = 0.0f;
        if (this._currentTrack != null)
          this.ShowNotification();
        else
          this.HideNotification();
        this.ZoomScaleFactor = 0;
      }
      if (flag3 != this._isContextCompatible)
      {
        this._isContextCompatible = flag3;
        this.FirePropertyChanged("IsPlaybackContextCompatible");
      }
      this.UpdateAvailabilityOfCommands();
      if (this.Playing && this.CurrentTrack != null && (this.CurrentTrack.IsVideo && this.CurrentTrack.IsStreaming))
      {
        this._isStreamingTimeoutTimer.Enabled = false;
        this.IsStreamingVideo = true;
        this.SupressDownloads = true;
      }
      else
      {
        if (this.SupressDownloads)
          this._isStreamingTimeoutTimer.Enabled = true;
        this.IsStreamingVideo = false;
      }
    }

    private void UpdateAvailabilityOfCommands()
    {
      bool playing = this.Playing;
      bool flag = this._playlistCurrent != null;
      this._play.Available = !playing && flag && !this.Buffering;
      this._pause.Available = playing && !this.Buffering;
      this._stop.Available = flag;
      if (this._playerState == TransportControls.PlayerState.Stopped || this.Buffering)
      {
        this._forward.Available = false;
        this._back.Available = false;
      }
      else if (this._playerState == TransportControls.PlayerState.Playing)
      {
        if (this._currentTrack == null)
        {
          this._forward.Available = false;
          this._back.Available = false;
        }
        else
        {
          this._forward.Available = !this._currentTrack.IsVideo || this._playbackWrapper.CanChangeVideoRate;
          this._back.Available = true;
        }
      }
      else
      {
        this._forward.Available = this._currentTrack == null ? this._playlistCurrent != null : this._playlistCurrent != null && !this._currentTrack.IsVideo;
        this._back.Available = this._playlistCurrent != null && this._playlistCurrent.CanRetreat;
      }
      this.UpdateTaskbarPlayer();
    }

    public static string FormatDuration(float seconds, bool prefixWithNegative)
    {
      return Shell.TimeSpanToString(new TimeSpan(0, 0, (int) seconds), prefixWithNegative);
    }

    public static string FormatDuration(float seconds)
    {
      return TransportControls.FormatDuration(seconds, false);
    }

    [Conditional("DEBUG_TRANSPORT")]
    private static void _DEBUG_Trace(string message, params object[] args)
    {
    }

    [Conditional("DEBUG_TRANSPORT_PROPERTIES")]
    private static void _DEBUG_TracePropChange(string name, object arg)
    {
    }

    private struct SpectrumOutputConfig
    {
      public uint SourceId;
      public uint NumBands;
      public bool Frequency;
      public bool Waveform;
      public bool Stereo;
      public bool IsConnected;
    }

    private enum PlayerState
    {
      Stopped,
      Playing,
      Paused,
    }
  }
}
