<?xml version = "1.0" encoding="UTF-8" standalone="yes"?>
<CPLEXSolution version="1.2">
 <header
   problemName="ILOG.CPLEX"
   solutionName="incumbent"
   solutionIndex="-1"
   objectiveValue="0.001"
   solutionTypeValue="3"
   solutionTypeString="primal"
   solutionStatusValue="101"
   solutionStatusString="integer optimal solution"
   solutionMethodString="mip"
   primalFeasible="1"
   dualFeasible="1"
   MIPNodes="0"
   MIPIterations="40"
   writeLevel="1"/>
 <quality
   epInt="1.0000000000000001e-05"
   epRHS="9.9999999999999995e-07"
   maxIntInfeas="0"
   maxPrimalInfeas="0"
   maxX="1"
   maxSlack="5"/>
 <linearConstraints>
  <constraint name="C1_0" index="0" slack="0"/>
  <constraint name="C1_1" index="1" slack="0"/>
  <constraint name="C1_2" index="2" slack="0"/>
  <constraint name="2_0_0_0_1" index="3" slack="-0.11365561285692038"/>
  <constraint name="2_0_0_0_2" index="4" slack="-2.0322837724966383"/>
  <constraint name="2_0_0_0_3" index="5" slack="-2.038566461782235"/>
  <constraint name="max_attributes" index="6" slack="5"/>
  <constraint name="2_0_0_1_1" index="7" slack="-2.6278332435189906"/>
  <constraint name="2_0_0_1_2" index="8" slack="-2.4068047070907461"/>
  <constraint name="max_attributes" index="9" slack="5"/>
  <constraint name="2_0_1_0_1" index="10" slack="-1.08143036599943"/>
  <constraint name="2_0_1_0_2" index="11" slack="-0.23994692908271575"/>
  <constraint name="2_0_1_0_3" index="12" slack="-1.0404873266320158"/>
  <constraint name="max_attributes" index="13" slack="5"/>
  <constraint name="2_0_1_1_1" index="14" slack="-0.84048343691671423"/>
  <constraint name="2_0_1_1_2" index="15" slack="-0.79954039754930006"/>
  <constraint name="max_attributes" index="16" slack="5"/>
  <constraint name="2_1_0_0_1" index="17" slack="-0.44945949010991448"/>
  <constraint name="2_1_0_0_2" index="18" slack="-0.40006358777175821"/>
  <constraint name="2_1_0_0_3" index="19" slack="-3.2082022566852171"/>
  <constraint name="max_attributes" index="20" slack="5"/>
  <constraint name="2_1_0_1_1" index="21" slack="-0.64791042038607838"/>
  <constraint name="2_1_0_1_2" index="22" slack="-3.2471519921660192"/>
  <constraint name="max_attributes" index="23" slack="5"/>
  <constraint name="2_1_1_0_1" index="24" slack="-0.2707309359096598"/>
  <constraint name="2_1_1_0_2" index="25" slack="-0.97231335114935069"/>
  <constraint name="2_1_1_0_3" index="26" slack="-0.24320461279535832"/>
  <constraint name="max_attributes" index="27" slack="5"/>
  <constraint name="2_1_1_1_1" index="28" slack="-0.026526323114301475"/>
  <constraint name="2_1_1_1_2" index="29" slack="-0.72810873835399237"/>
  <constraint name="max_attributes" index="30" slack="5"/>
  <constraint name="2_2_0_0_1" index="31" slack="-0.86222670081175323"/>
  <constraint name="2_2_0_0_2" index="32" slack="-1.7092568854378767"/>
  <constraint name="2_2_0_0_3" index="33" slack="-1.0978622192506968"/>
  <constraint name="max_attributes" index="34" slack="5"/>
  <constraint name="2_2_0_1_1" index="35" slack="-2.9441917352118478"/>
  <constraint name="2_2_0_1_2" index="36" slack="-0.60834366740116153"/>
  <constraint name="max_attributes" index="37" slack="5"/>
  <constraint name="2_2_1_0_1" index="38" slack="-0.61404153214164126"/>
  <constraint name="2_2_1_0_2" index="39" slack="-0.18535407448110908"/>
  <constraint name="2_2_1_0_3" index="40" slack="-0.2974199587015528"/>
  <constraint name="max_attributes" index="41" slack="5"/>
  <constraint name="2_2_1_1_1" index="42" slack="-0.42768745766053229"/>
  <constraint name="2_2_1_1_2" index="43" slack="-0.11106588422044383"/>
  <constraint name="max_attributes" index="44" slack="5"/>
  <constraint name="c46" index="45" slack="0"/>
  <constraint name="c47" index="46" slack="0"/>
  <constraint name="c48" index="47" slack="0"/>
  <constraint name="c49" index="48" slack="-2"/>
  <constraint name="c50" index="49" slack="0"/>
  <constraint name="c51" index="50" slack="0"/>
 </linearConstraints>
 <variables>
  <variable name="s_2_1" index="0" value="0"/>
  <variable name="s_2_0" index="1" value="1"/>
  <variable name="s_0_1" index="2" value="0"/>
  <variable name="s_0_0" index="3" value="1"/>
  <variable name="s_1_1" index="4" value="0"/>
  <variable name="s_1_0" index="5" value="1"/>
  <variable name="g_0" index="6" value="0"/>
  <variable name="g_1" index="7" value="1"/>
  <variable name="g_2" index="8" value="0"/>
  <variable name="gamma_2_1_0" index="9" value="0"/>
  <variable name="a_0" index="10" value="0"/>
  <variable name="a_1" index="11" value="1"/>
  <variable name="a_2" index="12" value="0"/>
  <variable name="gamma_2_1_1" index="13" value="0"/>
  <variable name="gamma_2_0_0" index="14" value="0"/>
  <variable name="gamma_2_0_1" index="15" value="0"/>
  <variable name="gamma_0_1_0" index="16" value="0"/>
  <variable name="gamma_0_1_1" index="17" value="0"/>
  <variable name="gamma_0_0_0" index="18" value="0"/>
  <variable name="gamma_0_0_1" index="19" value="0"/>
  <variable name="gamma_1_1_0" index="20" value="0"/>
  <variable name="gamma_1_1_1" index="21" value="0"/>
  <variable name="gamma_1_0_0" index="22" value="0"/>
  <variable name="gamma_1_0_1" index="23" value="0"/>
 </variables>
</CPLEXSolution>
