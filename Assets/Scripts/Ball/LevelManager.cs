public class LevelManager : Singleton<LevelManager>{
    
    private bool lateStart;
    void Update() {
        if(!lateStart){ // Activates only once, frame after start
            lateStart = true;
            // This one needs to be done after start since the thing its getting is created in start but can't be moved to awake
            PredictionManager.instance.copyAllObstacles(); 
        }
    }
}
