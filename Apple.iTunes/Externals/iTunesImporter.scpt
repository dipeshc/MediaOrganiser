FasdUAS 1.101.10   ��   ��    k             l        	  j     �� 
�� (0 delay_time_seconds DELAY_TIME_SECONDS 
 m     ����   3 - How long to wait between checking file size.    	 �   Z   H o w   l o n g   t o   w a i t   b e t w e e n   c h e c k i n g   f i l e   s i z e .      l     ��������  ��  ��     ��  i        I     ��  
�� .facofgetnull���     alis  o      ���� 0 
thisfolder 
thisFolder  �� ��
�� 
flst  o      ���� 0 theitems theItems��    X     h ��   k    c       r        m    ����    o      ���� 0 oldsize oldSize      r        m    ������  o      ���� 0 newsize newSize       l   �� ! "��   ! b \ When newSize equals oldSize, it means the copy is complete because the size hasn't changed.    " � # # �   W h e n   n e w S i z e   e q u a l s   o l d S i z e ,   i t   m e a n s   t h e   c o p y   i s   c o m p l e t e   b e c a u s e   t h e   s i z e   h a s n ' t   c h a n g e d .    $ % $ V    B & ' & k     = ( (  ) * ) l     �� + ,��   +   Get the file size.    , � - - &   G e t   t h e   f i l e   s i z e . *  . / . r     ) 0 1 0 n     ' 2 3 2 1   % '��
�� 
ptsz 3 l    % 4���� 4 I    %�� 5��
�� .sysonfo4asfe        file 5 o     !���� 0 f  ��  ��  ��   1 o      ���� 0 oldsize oldSize /  6 7 6 I  * 3�� 8��
�� .sysodelanull��� ��� nmbr 8 o   * /���� (0 delay_time_seconds DELAY_TIME_SECONDS��   7  9 : 9 l  4 4�� ; <��   ; 8 2 Sample the size again after delay for comparison.    < � = = d   S a m p l e   t h e   s i z e   a g a i n   a f t e r   d e l a y   f o r   c o m p a r i s o n . :  >�� > r   4 = ? @ ? n   4 ; A B A 1   9 ;��
�� 
ptsz B l  4 9 C���� C I  4 9�� D��
�� .sysonfo4asfe        file D o   4 5���� 0 f  ��  ��  ��   @ o      ���� 0 newsize newSize��   ' >    E F E o    ���� 0 newsize newSize F o    ���� 0 oldsize oldSize %  G H G l  C C��������  ��  ��   H  I J I l  C C�� K L��   K , & HERE BEGINS THE ITUNES SPECIFIC STUFF    L � M M L   H E R E   B E G I N S   T H E   I T U N E S   S P E C I F I C   S T U F F J  N O N O   C a P Q P k   G ` R R  S T S I  G L������
�� .ascrnoop****      � ****��  ��   T  U�� U Q   M ` V W�� V k   P W X X  Y Z Y I  P U�� [��
�� .hookAdd cTrk      @ alis [ o   P Q���� 0 f  ��   Z  \�� \ l   V V�� ] ^��   ] � � UNCOMMENT OUT NEXT 2 LINES IF YOU WANT THE FILE REMOVED AFTER IMPORT
				set the file_path to the quoted form of the POSIX path of f
				do shell script ("rm -f " & file_path)
                            ^ � _ _�   U N C O M M E N T   O U T   N E X T   2   L I N E S   I F   Y O U   W A N T   T H E   F I L E   R E M O V E D   A F T E R   I M P O R T 
 	 	 	 	 s e t   t h e   f i l e _ p a t h   t o   t h e   q u o t e d   f o r m   o f   t h e   P O S I X   p a t h   o f   f 
 	 	 	 	 d o   s h e l l   s c r i p t   ( " r m   - f   "   &   f i l e _ p a t h ) 
                                                ��   W R      ������
�� .ascrerr ****      � ****��  ��  ��  ��   Q m   C D ` `�                                                                                  hook  alis    N  Macintosh HD               �MSH+     �
iTunes.app                                                      b*���        ����  	                Applications    �L�c      �k�       �  %Macintosh HD:Applications: iTunes.app    
 i T u n e s . a p p    M a c i n t o s h   H D  Applications/iTunes.app   / ��   O  a b a l  b b�� c d��   c * $ HERE ENDS THE ITUNES SPECIFIC STUFF    d � e e H   H E R E   E N D S   T H E   I T U N E S   S P E C I F I C   S T U F F b  f�� f l  b b��������  ��  ��  ��  �� 0 f    o    ���� 0 theitems theItems��       �� g�� h��   g ������ (0 delay_time_seconds DELAY_TIME_SECONDS
�� .facofgetnull���     alis��  h �� ���� i j��
�� .facofgetnull���     alis�� 0 
thisfolder 
thisFolder�� ������
�� 
flst�� 0 theitems theItems��   i ������������ 0 
thisfolder 
thisFolder�� 0 theitems theItems�� 0 f  �� 0 oldsize oldSize�� 0 newsize newSize j ������������ `��������
�� 
kocl
�� 
cobj
�� .corecnte****       ****
�� .sysonfo4asfe        file
�� 
ptsz
�� .sysodelanull��� ��� nmbr
�� .ascrnoop****      � ****
�� .hookAdd cTrk      @ alis��  ��  �� i g�[��l kh jE�OiE�O )h���j �,E�Ob   j O�j �,E�[OY��O� *j O �j OPW X 	 
hUOP[OY��ascr  ��ޭ