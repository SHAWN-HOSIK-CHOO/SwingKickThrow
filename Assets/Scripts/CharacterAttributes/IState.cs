namespace CharacterAttributes
{
   public interface IState
   {
      void Setup(CharacterFSMContext inputContext, CharacterControllerNetwork inputOwner);
      void OnEnter();
      void OnUpdate();
      void OnExit();
   }
}
