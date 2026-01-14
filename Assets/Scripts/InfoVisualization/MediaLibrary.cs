using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public static class MediaLibrary
{
    public static List<MediaItem> LoadAll()
    {
        var list = new List<MediaItem>();

        // Images
        var images = Resources.LoadAll<Texture2D>("AltarMedia/Images");
        foreach (var tex in images)
        {
            list.Add(new MediaItem {
                id = tex.name,
                type = MediaType.Image,
                preview = tex,
                image = tex
            });
        }

        // Videos + posters
        var videos = Resources.LoadAll<VideoClip>("AltarMedia/Videos");
        foreach (var clip in videos)
        {
            // On cherche une image de même nom : intro.mp4 => intro.png
            var poster = Resources.Load<Texture2D>($"AltarMedia/Videos/{clip.name}");
            // poster peut être null si tu l’as oublié => on laisse null, tu peux mettre un placeholder
            list.Add(new MediaItem {
                id = clip.name,
                type = MediaType.Video,
                preview = poster,
                video = clip
            });
        }

        // Tri optionnel
        return list.OrderBy(m => m.type).ThenBy(m => m.id).ToList();
    }
}
