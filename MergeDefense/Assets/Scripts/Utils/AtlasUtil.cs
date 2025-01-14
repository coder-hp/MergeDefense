using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasUtil
{
    static SpriteAtlas atlas_main = null;
    static SpriteAtlas atlas_game = null;
    static SpriteAtlas atlas_set = null;
    static SpriteAtlas atlas_loading = null;
    static SpriteAtlas atlas_shopIcon = null;
    static SpriteAtlas atlas_island = null;

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

    public static SpriteAtlas getAtlas_shopIcon()
    {
        if (atlas_shopIcon == null)
        {
            atlas_shopIcon = Resources.Load("Atlas/shopIcon", typeof(SpriteAtlas)) as SpriteAtlas;
        }
        return atlas_shopIcon;
    }

    public static SpriteAtlas getAtlas_loading()
    {
        if (atlas_loading == null)
        {
            atlas_loading = Resources.Load("Atlas/loading", typeof(SpriteAtlas)) as SpriteAtlas;
        }
        return atlas_loading;
    }

    public static SpriteAtlas getAtlas_set()
    {
        if (atlas_set == null)
        {
            atlas_set = Resources.Load("Atlas/set", typeof(SpriteAtlas)) as SpriteAtlas;
        }
        return atlas_set;
    }

    public static SpriteAtlas getAtlas_island()
    {
        if (atlas_island == null)
        {
            atlas_island = Resources.Load("Atlas/island", typeof(SpriteAtlas)) as SpriteAtlas;
        }
        return atlas_island;
    }
}
