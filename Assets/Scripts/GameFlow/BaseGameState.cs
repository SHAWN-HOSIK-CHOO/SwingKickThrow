namespace GameFlow
{
   public interface IGameState
   {
      void SetUp(GameSceneFlowContext gameSceneFlowContext);
      void Handle();
      void OnExit();
      void Finish(GameStateType type);
   }
   
   public abstract class BaseGameState : IGameState
   {
      protected GameSceneFlowContext    Context;
      
      public virtual void SetUp(GameSceneFlowContext gameSceneFlowContext)
      {
         Context = gameSceneFlowContext;
      }

      public abstract void LateSetUp();
      
      public abstract void Handle();

      public abstract void OnExit();

      public virtual void Finish(GameStateType type)
      {
         Context.FinishState(type);
      }
   }
}
