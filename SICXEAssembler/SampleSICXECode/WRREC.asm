	WRREC	CSECT
	.
	.	SUBROUTINE TO WRITE RECORD FROM BUFFER
	.
		EXTREF	LENGTH,BUFFER
		CLEAR 	X
		+LDT	LENGTH
	WLOOP	TD	=X'05'
		JEQ	WLOOP
		+LDCH	BUFFER,X
		WD	=X'05'
		TIXR	T
		JLT	WLOOP
		RSUB
		END