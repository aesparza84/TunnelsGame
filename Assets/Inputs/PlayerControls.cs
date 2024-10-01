//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Inputs/PlayerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Crawl"",
            ""id"": ""2df64cf5-14bb-4ca8-8fb4-e989aa893e1e"",
            ""actions"": [
                {
                    ""name"": ""LeftArm"",
                    ""type"": ""Button"",
                    ""id"": ""cbc558c8-c6c6-4a4a-85c1-13d1306ba50c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightArm"",
                    ""type"": ""Button"",
                    ""id"": ""21896bff-87e7-4133-be7c-26b6b63931a6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CamLeft"",
                    ""type"": ""Button"",
                    ""id"": ""8ce0d482-0f87-474f-b95f-cb71ad83d6b4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CamRight"",
                    ""type"": ""Button"",
                    ""id"": ""901831c1-30f1-453e-b91c-15e629aaf31b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7be1254e-04c7-4c35-977a-abcab8112fe3"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftArm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""09f5f53e-66fd-49c0-82fe-d49df49b3775"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightArm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5f891f6d-34e0-488a-9e18-c4b2d1dc098e"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CamLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e36722d0-9910-4b9e-8292-54df1f845a4f"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CamRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Crawl
        m_Crawl = asset.FindActionMap("Crawl", throwIfNotFound: true);
        m_Crawl_LeftArm = m_Crawl.FindAction("LeftArm", throwIfNotFound: true);
        m_Crawl_RightArm = m_Crawl.FindAction("RightArm", throwIfNotFound: true);
        m_Crawl_CamLeft = m_Crawl.FindAction("CamLeft", throwIfNotFound: true);
        m_Crawl_CamRight = m_Crawl.FindAction("CamRight", throwIfNotFound: true);
    }

    public void Dispose()
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Crawl
    private readonly InputActionMap m_Crawl;
    private List<ICrawlActions> m_CrawlActionsCallbackInterfaces = new List<ICrawlActions>();
    private readonly InputAction m_Crawl_LeftArm;
    private readonly InputAction m_Crawl_RightArm;
    private readonly InputAction m_Crawl_CamLeft;
    private readonly InputAction m_Crawl_CamRight;
    public struct CrawlActions
    {
        private @PlayerControls m_Wrapper;
        public CrawlActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftArm => m_Wrapper.m_Crawl_LeftArm;
        public InputAction @RightArm => m_Wrapper.m_Crawl_RightArm;
        public InputAction @CamLeft => m_Wrapper.m_Crawl_CamLeft;
        public InputAction @CamRight => m_Wrapper.m_Crawl_CamRight;
        public InputActionMap Get() { return m_Wrapper.m_Crawl; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CrawlActions set) { return set.Get(); }
        public void AddCallbacks(ICrawlActions instance)
        {
            if (instance == null || m_Wrapper.m_CrawlActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_CrawlActionsCallbackInterfaces.Add(instance);
            @LeftArm.started += instance.OnLeftArm;
            @LeftArm.performed += instance.OnLeftArm;
            @LeftArm.canceled += instance.OnLeftArm;
            @RightArm.started += instance.OnRightArm;
            @RightArm.performed += instance.OnRightArm;
            @RightArm.canceled += instance.OnRightArm;
            @CamLeft.started += instance.OnCamLeft;
            @CamLeft.performed += instance.OnCamLeft;
            @CamLeft.canceled += instance.OnCamLeft;
            @CamRight.started += instance.OnCamRight;
            @CamRight.performed += instance.OnCamRight;
            @CamRight.canceled += instance.OnCamRight;
        }

        private void UnregisterCallbacks(ICrawlActions instance)
        {
            @LeftArm.started -= instance.OnLeftArm;
            @LeftArm.performed -= instance.OnLeftArm;
            @LeftArm.canceled -= instance.OnLeftArm;
            @RightArm.started -= instance.OnRightArm;
            @RightArm.performed -= instance.OnRightArm;
            @RightArm.canceled -= instance.OnRightArm;
            @CamLeft.started -= instance.OnCamLeft;
            @CamLeft.performed -= instance.OnCamLeft;
            @CamLeft.canceled -= instance.OnCamLeft;
            @CamRight.started -= instance.OnCamRight;
            @CamRight.performed -= instance.OnCamRight;
            @CamRight.canceled -= instance.OnCamRight;
        }

        public void RemoveCallbacks(ICrawlActions instance)
        {
            if (m_Wrapper.m_CrawlActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ICrawlActions instance)
        {
            foreach (var item in m_Wrapper.m_CrawlActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_CrawlActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public CrawlActions @Crawl => new CrawlActions(this);
    public interface ICrawlActions
    {
        void OnLeftArm(InputAction.CallbackContext context);
        void OnRightArm(InputAction.CallbackContext context);
        void OnCamLeft(InputAction.CallbackContext context);
        void OnCamRight(InputAction.CallbackContext context);
    }
}
