using UnityEngine;
using UnityEngine.Video;

public enum MediaType { Image, Video }

[System.Serializable]
public class MediaItem
{
    public string id;                // ex: "intro"
    public MediaType type;

    public Texture2D preview;         // thumbnail affichée dans le menu

    public Texture2D image;           // si type Image
    public VideoClip video;           // si type Video
}
