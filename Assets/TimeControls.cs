// GENERATED AUTOMATICALLY FROM 'Assets/TimeControls.inputactions'

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class TimeControls : IInputActionCollection
{
    private InputActionAsset asset;
    public TimeControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""TimeControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""df7fd660-37fb-4de2-9190-3742547db6bd"",
            ""actions"": [
                {
                    ""name"": ""Rewind"",
                    ""type"": ""Button"",
                    ""id"": ""6a4a1d57-eb0e-43cf-be03-85079968625e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""90d387c3-834c-41da-8367-cbc5dec78f37"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""866d1e1a-96a7-4b52-9a08-0ce3a45af933"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9b9f36f7-d1b6-4746-8a1a-818fbafa0336"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rewind"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.GetActionMap("Gameplay");
        m_Gameplay_Rewind = m_Gameplay.GetAction("Rewind");
        m_Gameplay_Shoot = m_Gameplay.GetAction("Shoot");
    }

    ~TimeControls()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Rewind;
    private readonly InputAction m_Gameplay_Shoot;
    public struct GameplayActions
    {
        private TimeControls m_Wrapper;
        public GameplayActions(TimeControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Rewind => m_Wrapper.m_Gameplay_Rewind;
        public InputAction @Shoot => m_Wrapper.m_Gameplay_Shoot;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                Rewind.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRewind;
                Rewind.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRewind;
                Rewind.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRewind;
                Shoot.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnShoot;
                Shoot.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnShoot;
                Shoot.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnShoot;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                Rewind.started += instance.OnRewind;
                Rewind.performed += instance.OnRewind;
                Rewind.canceled += instance.OnRewind;
                Shoot.started += instance.OnShoot;
                Shoot.performed += instance.OnShoot;
                Shoot.canceled += instance.OnShoot;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnRewind(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
    }
}
