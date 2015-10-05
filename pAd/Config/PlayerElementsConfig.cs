
using System.Drawing;



namespace Pingvi {
    public class PlayerElementsConfig {

        public Rectangle Card1Rect { get; set; }
        public Rectangle Card2Rect { get; set; }

        public PixelPoint PlayerTypePoint { get; set; }
        public PixelPoint PlayerStatusPointGame { get; set; }
        public PixelPoint PlayerStatusPointHand { get; set; }
        public PixelPoint PlayerStatusPointSitOut { get; set; }
        public PixelPoint ButtonPoint { get; set; }

        public PixelPoint[] StackDigPosPoints { get; set; }
        public Rectangle[][] StackDigitsRectMass { get; set; }

        public PixelPoint[] BetDigPosPoints { get; set; }
        public Rectangle[][] BetDigitsRectMass { get; set; }



        //LINE

        public PixelPoint[] LinePixelPositions { get; set; }
        //ALTER LINE
        public Rectangle[] LineRectPosition { get; set; }
      

        //STATS
        //FIRST PANEL
        public PixelPoint[] PF_BTN_STEAL_StatDigPosPoints { get; set; }
        public Rectangle[][] PF_BTN_STEAL_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_SB_STEAL_StatDigPosPoints { get; set; }
        public Rectangle[][] PF_SB_STEAL_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_OPENPUSH_StatDigPosPoints { get; set; }
        public Rectangle[][] PF_OPENPUSH_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_LIMP_FOLD_StatDigPosPoints { get; set; }
        public Rectangle[][] PF_LIMP_FOLD_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_LIMP_RERAISE_StatDigPosPoints { get; set; }
        public Rectangle[][] PF_LIMP_RERAISE_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_FOLD_3BET_StatDigPosPoints { get; set; }
        public Rectangle[][] PF_FOLD_3BET_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_BB_DEF_VS_SBSTEAL_StatDigPosPoints { get; set; }
        public Rectangle[][] PF_BB_DEF_VS_SBSTEAL_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_RAISE_LIMPER_StatDigPosPoints { get; set; }
        public Rectangle[][] PF_RAISE_LIMPER_StatDigitsRectMass { get; set; }
        
        public PixelPoint[] PF_SB_3BET_VS_BTN_StatDigPosPoints { get; set; }
        public Rectangle[][] PF_SB_3BET_VS_BTN_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_BB_3BET_VS_BTN_StatDigPosPoints { get; set; }
        public Rectangle[][] PF_BB_3BET_VS_BTN_StatDigitsRectMass { get; set; }

        public PixelPoint[] PF_BB_3BET_VS_SB_StatDigPosPoints { get; set; }
        public Rectangle[][] PF_BB_3BET_VS_SB_StatDigitsRectMass { get; set; }

        //SECOND PANEL
        public PixelPoint[] F_CBET_StatDigPosPoints { get; set; }
        public Rectangle[][] F_CBET_StatDigitsRectMass { get; set; }

        public PixelPoint[] T_CBET_StatDigPosPoints { get; set; }
        public Rectangle[][] T_CBET_StatDigitsRectMass { get; set; }

        public PixelPoint[] R_CBET_StatDigPosPoints { get; set; }
        public Rectangle[][] R_CBET_StatDigitsRectMass { get; set; }

        public PixelPoint[] F_FOLD_CBET_StatDigPosPoints { get; set; }
        public Rectangle[][] F_FOLD_CBET_StatDigitsRectMass { get; set; }

        public PixelPoint[] T_FOLD_CBET_StatDigPosPoints { get; set; }
        public Rectangle[][] T_FOLD_CBET_StatDigitsRectMass { get; set; }

        public PixelPoint[] R_FOLD_CBET_StatDigPosPoints { get; set; }
        public Rectangle[][] R_FOLD_CBET_StatDigitsRectMass { get; set; }

        public PixelPoint[] F_CBET_FOLDRAISE_StatDigPosPoints { get; set; }
        public Rectangle[][] F_CBET_FOLDRAISE_StatDigitsRectMass { get; set; }

        public PixelPoint[] T_CBET_FOLDRAISE_StatDigPosPoints { get; set; }
        public Rectangle[][] T_CBET_FOLDRAISE_StatDigitsRectMass { get; set; }

        public PixelPoint[] F_RAISE_BET_StatDigPosPoints { get; set; }
        public Rectangle[][] F_RAISE_BET_StatDigitsRectMass { get; set; }

        public PixelPoint[] T_RAISE_BET_StatDigPosPoints { get; set; }
        public Rectangle[][] T_RAISE_BET_StatDigitsRectMass { get; set; }

        public PixelPoint[] F_LP_STEAL_StatDigPosPoints { get; set; }
        public Rectangle[][] F_LP_STEAL_StatDigitsRectMass { get; set; }

        //THIRD PANEL
        public PixelPoint[] F_LP_FOLD_VS_STEAL_StatDigPosPoints { get; set; }
        public Rectangle[][] F_LP_FOLD_VS_STEAL_StatDigitsRectMass { get; set; }

        public PixelPoint[] F_LP_FOLD_VS_XR_StatDigPosPoints { get; set; }
        public Rectangle[][] F_LP_FOLD_VS_XR_StatDigitsRectMass { get; set; }

        public PixelPoint[] F_CHECKFOLD_OOP_StatDigPosPoints { get; set; }
        public Rectangle[][] F_CHECKFOLD_OOP_StatDigitsRectMass { get; set; }

        public PixelPoint[] T_SKIPF_FOLD_VS_T_PROBE_StatDigPosPoints { get; set; }
        public Rectangle[][] T_SKIPF_FOLD_VS_T_PROBE_StatDigitsRectMass { get; set; }

        public PixelPoint[] R_SKIPT_FOLD_VS_R_PROBE_StatDigPosPoints { get; set; }
        public Rectangle[][] R_SKIPT_FOLD_VS_R_PROBE_StatDigitsRectMass { get; set; }

        public PixelPoint[] F_DONK_StatDigPosPoints { get; set; }
        public Rectangle[][] F_DONK_StatDigitsRectMass { get; set; }

        public PixelPoint[] T_DONK_StatDigPosPoints { get; set; }
        public Rectangle[][] T_DONK_StatDigitsRectMass { get; set; }

        public PixelPoint[] F_DONK_FOLDRAISE_StatDigPosPoints { get; set; }
        public Rectangle[][] F_DONK_FOLDRAISE_StatDigitsRectMass { get; set; }
   
  
      
 
 
        
    
     
    

       

    }
}
