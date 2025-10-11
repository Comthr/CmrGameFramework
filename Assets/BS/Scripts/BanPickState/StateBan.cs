using CmrGame;
using CmrGameFramework;
using CmrGameFramework.DataTable;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using StateOwner = CmrGameFramework.Fsm.IFsm<BSTool.IBanPickHandler>;

namespace BSTool
{
    public class StateBan : BanPickStateBase
    {
        private int m_State;
        private bool m_CanNext;
        private List<int> selections;
        protected override void OnEnter(StateOwner procedureOwner)
        {
            m_State = 0;

            //波
        }

        protected override void OnLeave(StateOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(StateOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        }
        private IEnumerator StateProcedure(StateOwner procedureOwner)
        {
            //蓝方ban两个英雄
            //这里要做的是先比对当前回合对UI进行更新
            //procedureOwner.Owner.IsBlue;

            yield return new WaitUntil(() => m_CanNext);


            //红方ban两个英雄
        }
        public void SelectOver(List<int> charas)
        {
            m_CanNext = true;
            selections = charas;
        }

    }

}
