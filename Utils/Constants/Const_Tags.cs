﻿using System.Collections.Generic;

public static class Const_Tags
{
    // Allows Tags to be exposed in Editor so we don't have to use strings as properties
    // when specifying tags.
    public enum Tags
    {
        None                            = 0,
        PlayerAnimator                  = 1,
        Player                          = 2,
        SavePoint                       = 3,
        Pushable                        = 4,
        InteractableObject              = 5,
        ItemObject                      = 6,
        UsableTarget                    = 7,
        DialogueChoice                  = 8,
        Puppet                          = 9,
        UniqueBlockingPuppetTop         = 10,
    }

    public static Dictionary<Tags, string> TagsMap = new Dictionary<Tags, string>
    {
        { Tags.None,                            null },
        { Tags.PlayerAnimator,                  PlayerAnimator },
        { Tags.Player,                          Player },
        { Tags.SavePoint,                       SavePoint },
        { Tags.Pushable,                        Pushable },
        { Tags.InteractableObject,              InteractableObject },
        { Tags.ItemObject,                      ItemObject },
        { Tags.UsableTarget,                    UsableTarget },
        { Tags.DialogueChoice,                  DialogueChoice },
        { Tags.Puppet,                          Puppet },
        { Tags.UniqueBlockingPuppetTop,         UniqueBlockingPuppetTop },
    };
    
    public const string PlayerAnimator = "tag_player-animator";
    public const string Player = "tag_player";
    public const string NPC = "tag_interactable_NPC";
    public const string SavePoint = "tag_interactable_save-point";
    public const string Pushable = "tag_interactable_pushable";
    public const string InteractableObject = "tag_interactable_interactable-object";
    public const string ItemObject = "tag_interactable_item-object";
    public const string UsableTarget = "tag_interactable_usable-target";
    public const string DialogueChoice = "tag_dialogue-choice-text";
    public const string Puppet = "tag_puppet";
    public const string UniqueBlockingPuppetTop = "tag_unique-blocking_puppet-top";

}
