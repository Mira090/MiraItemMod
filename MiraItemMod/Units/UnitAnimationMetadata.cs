using FMODUnity;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Units
{
    public class UnitAnimationMetadata
    {
        public string name { get; set; }
        public List<AnimationDatum> animationData { get; set; }
        public class AnimationDatum
        {
            public string state { get; set; }
            public List<Timeline> timeline { get; set; }
            public bool repeat { get; set; }
            public int fps { get; set; }
            public List<FrameEvent> frameEvents { get; set; }
            public List<SoundEvent> soundEvents { get; set; }
        }
        public class Timeline
        {
            public int frameIdx { get; set; }
            public string sprite { get; set; }
        }

        public class FrameEvent
        {
            public int frame { get; set; }
            public List<Event> events { get; set; }
        }
        public class SoundEvent
        {
            public int frame { get; set; }
            public List<SEvent> events { get; set; }
        }
        public class Event
        {
            public string componentName { get; set; }
            public string methodName { get; set; }
            public string priority { get; set; }
        }
        public class SEvent
        {
            public string path { get; set; }
            public bool attachToPerformer { get; set; }
        }

        public List<AnimationSet.StateInfo> ToList(string name)
        {
            var state = new List<AnimationSet.StateInfo>();
            foreach (var animation in animationData)
            {
                var info = new AnimationSet.StateInfo();
                info.state = animation.state;
                info.fps = animation.fps;
                info.repeat = animation.repeat;

                info.timeline = new List<AnimationSet.StateInfo.SpriteKeyFrame>();
                foreach (var time in animation.timeline)
                {
                    var timeline = new AnimationSet.StateInfo.SpriteKeyFrame();
                    timeline.frameIdx = time.frameIdx;
                    timeline.sprite = AssetLoader.LoadSpriteForUnit(Path.Combine(ModUtil.UnitPath, name, time.sprite));
                    info.timeline.Add(timeline);
                }

                if (animation.frameEvents != null)
                {
                    info.frameEvents = new List<AnimationSet.StateInfo.FrameEvent>();
                    foreach (var frame in animation.frameEvents)
                    {
                        var frameEvent = new AnimationSet.StateInfo.FrameEvent();
                        frameEvent.frame = frame.frame;

                        frameEvent.events = new List<AnimationSet.StateInfo.Event>();
                        foreach (var ev in frame.events)
                        {
                            var even = new AnimationSet.StateInfo.Event();
                            even.methodName = ev.methodName;
                            even.componentName = ev.componentName;
                            even.priority = ev.priority == "Essential" ? AnimationSet.StateInfo.Event.EPriority.Essential : AnimationSet.StateInfo.Event.EPriority.Ignorable;
                            frameEvent.events.Add(even);
                        }
                        info.frameEvents.Add(frameEvent);
                    }
                }

                if (animation.soundEvents != null)
                {
                    info.soundEvents = new List<AnimationSet.StateInfo.FrameSoundEvent>();
                    foreach (var sound in animation.soundEvents)
                    {
                        var soundEvent = new AnimationSet.StateInfo.FrameSoundEvent();
                        soundEvent.frame = sound.frame;
                        soundEvent.events = new List<AnimationSet.StateInfo.SoundEvent>();
                        foreach (var ev in sound.events)
                        {
                            var even = new AnimationSet.StateInfo.SoundEvent();
                            even.attachToPerformer = ev.attachToPerformer;
                            even.path = RuntimeManager.PathToEventReference(ev.path);
                            soundEvent.events.Add(even);
                        }
                        info.soundEvents.Add(soundEvent);
                    }
                }
                state.Add(info);
            }
            return state;
        }
        public AnimationSet CreateAnimationSet(string name)
        {
            var set = ScriptableObject.CreateInstance<AnimationSet>();
            set.name = name;
            set.sprites = ToList(name);
            return set;
        }
    }
}
