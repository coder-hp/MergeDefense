using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasUtil
{
    static SpriteAtlas atlas_main = null;
    static SpriteAtlas atlas_game = null;
    static SpriteAtlas atlas_icon = null;

    public static SpriteAtlas getAtlas_main()
    {
        if (atlas_main == null)
        {
            atlas_main = Resources.Load("Atlas/main", typeof(SpriteAtlas)) as SpriteAtlas;
        }
        return atlas_main;
    }
    
    public static SpriteAtlas getAtlas_game()
    {
        if (atlas_game == null)
        {
            atlas_game = Resources.Load("Atlas/game", typeof(SpriteAtlas)) as SpriteAtlas;
        }
        return atlas_game;
    }

    public static SpriteAtlas getAtlas_icon()
    {
        if (atlas_icon == null)
        {
            atlas_icon = Resources.Load("Atlas/icon", typeof(SpriteAtlas)) as SpriteAtlas;
        }
        return atlas_icon;
    }
}
