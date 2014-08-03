using MPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPlayer.Services
{
    public class TransportControlService
    {
        public enum PlayerState
        {
            Paused,
            Playing,
            Stopped
        }

        public bool Buffering { get; private set; }
        public bool Playing { get; private set; }

        //public List<SongModel> CurrentPlaylist { get; set; }

        public SongModel CurrentTrack { get; private set; }

        public float CurrentTrackDuration { get; private set; }

        public float CurrentTrackIndex { get; private set; }

        public float CurrentTrackPosition { get; private set; }

        public float Volume { get; private set; }

        public void AddToNowPlaying()
        {
        }

        public void GetNextTracks()
        {
        }

        public void IsCurrentTrack()
        {
        }

        public void PlayItem()
        {
        }

        public void PlayItems()
        {
        }

        public void PlayPendingList()
        {
        }

        public void RemoveFromNowPlaying()
        {
        }

        public void ReorderNowPlaying()
        {
        }

        public void SeekToPosition(float value)
        {
        }

        public void SetPlayerState(PlayerState newState)
        {
        }

        public void StartPlayingAt()
        {
        }
    }
}
