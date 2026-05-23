using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Registries
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<CustomCostumeMetadata>(myJsonResponse);
    public class CustomCostumeMetadata
    {
        public string comment { get; set; }
        public string id { get; set; }
        public string costumeName { get; set; }
        public string costumeFlavorText { get; set; }
        public int hitboxSize { get; set; }
        public double movementCollisionRadius { get; set; }
        public bool hasBackImage { get; set; }
        public List<string> stats { get; set; }
        public List<int> startingItems { get; set; }
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
    }
}
