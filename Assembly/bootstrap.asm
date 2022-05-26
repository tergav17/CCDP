; --- BOOTSTRAP ---

	ld		bl,01
	st		bl,(0xF203)
	ld		b,0x0200
	xfr		b,s
loop:
	ld		al,(0xF202)
	srr		al
	bnl		loop
	ld		al,(0xF203)
	st		al,(b)
	inr		bl
	bnz		loop
	
