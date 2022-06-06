#include <stdio.h>
#include <stdlib.h>

/*
 * Original algorithm from WIPL bootloader:
 *
 * 0266:    95 88 06     ld AX, +0x6(EX)	 ; Disk code is derived from this value
 * 0269:    3b           not! AX
 * 026a:    c0 80        ld BL, #0x80	 ; This whole thing rotates AX right WITHOUT carry
 * 026c:    07           rl
 * 026d:    36 00        rrc AX, 1
 * 026f:    11 02        bnc L_0273
 * 0271:    43 30        or AH, BL
 * 
 * L_0273:
 * 0273:    d0 3c b1     ld BX, #0x3cb1	 ; Some more obfuscation
 * 0276:    44 32        xor BH, BL
 * 0278:    54 02        xor BX, AX	 ; BX is the final expected value here
 */
static unsigned short make_key(unsigned short seed)
{
    unsigned short AX = ~seed;
    unsigned short rotated = (AX >> 1) | (AX << 15);
    unsigned short BH = 0x3c ^ 0xb1;
    unsigned short BX = (BH << 8) | 0xb1;
    
    return BX ^ rotated;
}

static void usage(const char *progname) {
    printf("Usage: %s [seed value]\n", progname);
    exit(255);
}

/*
 * This quickly hacked up tool computes a disk key from seed value, which can be
 * retrieved from a raw disk dump.
 * The sector you need is cylinder 0, head 0, sector 14. The seed value is
 * two bytes at offset 6 (bigendian).
 * The correct sector also contains a value of 0xff at offset 8. If this value is
 * different the WIPL reports "INVALID DISK FORMAT" error.
 *
 * A quick test (using the seed from the first disk we cracked):
 * $ ./keygen.exe 0xe454
 * DISK CODE: 100
 */
int main(int argc, const char **argv) {
    char *end = NULL;
    unsigned short seed;

    if (argc < 2) {
	usage(argv[0]);
    }

    seed = strtoul(argv[1], &end, 0);
    
    if (*end) {
	printf("Invalid value supplied\n");
	usage(argv[0]);
	return 255;
    }
    
    printf("DISK CODE: %d\n", make_key(seed));
    return 0;
}
