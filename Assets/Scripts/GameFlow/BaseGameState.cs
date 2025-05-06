namespace GameFlow
{
   public interface IGameState
   {
      void SetUp(GameSceneManager gameSceneManager, GameSceneFlowContext gameSceneFlowContext);
      void Handle();
      void OnExit();
      void Finish(GameStateType type);
   }
   
   public abstract class BaseGameState : IGameState
   {
      protected GameSceneManager Owner;
      protected GameSceneFlowContext    Context;
      
      public virtual void SetUp(GameSceneManager gameSceneManager, GameSceneFlowContext gameSceneFlowContext)
      {
         Owner   = gameSceneManager;
         Context = gameSceneFlowContext;
      }

      public abstract void Handle();

      public abstract void OnExit();

      public virtual void Finish(GameStateType type)
      {
         Context.FinishState(type);
      }
   }
}
