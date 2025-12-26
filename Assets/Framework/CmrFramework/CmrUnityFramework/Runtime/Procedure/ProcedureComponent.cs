using CmrGameFramework;
using CmrGameFramework.Fsm;
using CmrGameFramework.Procedure;
using System;
using System.Collections;
using UnityEngine;
namespace CmrUnityFramework.Runtime
{
    /// <summary>
    /// 流程组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("CmrFramework/Procedure")]
    public sealed class ProcedureComponent : GameFrameworkComponent
    {
        private IProcedureManager m_ProcedureManager = null;
        private ProcedureBase m_EntranceProcedure = null;

        [SerializeField]
        private string[] m_AvailableProcedureTypeNames = null;

        [SerializeField]
        private string m_EntranceProcedureTypeName = null;

        /// <summary>
        /// 获取当前流程。
        /// </summary>
        public ProcedureBase CurrentProcedure
        {
            get
            {
                return m_ProcedureManager.CurrentProcedure;
            }
        }

        /// <summary>
        /// 获取当前流程持续时间。
        /// </summary>
        public float CurrentProcedureTime
        {
            get
            {
                return m_ProcedureManager.CurrentProcedureTime;
            }
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_ProcedureManager = GameFrameworkEntry.GetModule<IProcedureManager>();
            if (m_ProcedureManager == null)
            {
                Log.Fatal("Procedure manager is invalid.");
                return;
            }
        }

        /// <summary>
        /// CYJ:根据m_AvailableProcedureTypeNames中的名称，反射流程类
        /// 然后对流程管理器初始化，开启入口流程
        /// CYJConfuse:为什么要用协程？
        /// </summary>
        /// <returns></returns>
        private IEnumerator Start()
        {
            /*
             * CYJStep1-6:
             * 这里其实是游戏具体逻辑的入口，这也是一个协程方法。
             * 大概就是为了确保Start函数是所有组件的Start方法中最后执行的。
             * 这里先是对所有可用的流程进行了实例化，并找到入口流程，然后将其交给模块类去初始化。
             * Initialize方法中就是获取到了fsmManager并调用他获得了一个流程状态机。
             * 最后开启入口流程的StartProcedure方法就是调用了fsm的start方法
             * 所以使用这个框架的入口就是，创建一个流程，并实现其内部逻辑吧。
             * ok就这么简单，去到下一个组件，ResourceComponent.Start→
             */
            ProcedureBase[] procedures = new ProcedureBase[m_AvailableProcedureTypeNames.Length];
            for (int i = 0; i < m_AvailableProcedureTypeNames.Length; i++)
            {
                Type procedureType = Utility.Assembly.GetType(m_AvailableProcedureTypeNames[i]);
                if (procedureType == null)
                {
                    Log.Error("Can not find procedure type '{0}'.", m_AvailableProcedureTypeNames[i]);
                    yield break;
                }

                procedures[i] = (ProcedureBase)Activator.CreateInstance(procedureType);
                if (procedures[i] == null)
                {
                    Log.Error("Can not create procedure instance '{0}'.", m_AvailableProcedureTypeNames[i]);
                    yield break;
                }

                if (m_EntranceProcedureTypeName == m_AvailableProcedureTypeNames[i])
                    m_EntranceProcedure = procedures[i];
            }

            if (m_EntranceProcedure == null)
            {
                Log.Error("Entrance procedure is invalid.");
                yield break;
            }

            m_ProcedureManager.Initialize(GameFrameworkEntry.GetModule<IFsmManager>(), procedures);

            yield return new WaitForEndOfFrame();

            m_ProcedureManager.StartProcedure(m_EntranceProcedure.GetType());
        }
    }
}
