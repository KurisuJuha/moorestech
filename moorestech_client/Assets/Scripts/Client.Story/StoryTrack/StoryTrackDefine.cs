namespace Client.Story.StoryTrack
{
    public class StoryTrackDefine
    {
        public static IStoryTrack GetStoryTrack(string key)
        {
            return key switch
            {
                "Camerawork" => new CameraworkTrack(),
                "Text" => new TextTrack(),
                "CharacterTransform" => new CharacterTransformTrack(),
                "CharacterMotion" => new CharacterMotionTrack(),
                "WaitTime" => new WaitTimeTrack(),
                "Transition" => new TransitionTrack(),
                _ => null
            };
        }
    }
}