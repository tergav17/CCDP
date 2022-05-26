# CCDP
A minimal diagnostic environment for Centurion computers

## Overview

The Centurion Computer Diagnostic Package, or CCDP, is a set of utilities that is meant to assist in running diagnostic and other service functions on EE200 compatable computers. It is inspired by the XXDP programs from the PDP-11 series of computers, and takes a lot of design idea from them. Overall, CCDP is useful for the following purposes:

  1. Easily sending large diagnostic and service programs to the Centurion.
  2. Giving programmers a very simple set of system calls to utilize in their programs
  3. Transfering large amounts of data to and from the Centurion over a serial connection.

CCDP currently makes use of a program called SerialDir to provide access to bootstrap information and a filesystem over a serial line. This makes it possible to boot CCDP without needing any working storage options on the Centurion. SerialDir works much like TU58FS, where the Centurion (client) computer is connected to a host computer running SerialDir over a serial line.

## Bootstrapping

### Part 1, Setup

When booting CCDP, the first thing that needs to be set up is the communication line between the host computer and the client computer. This will involve 2 serial connections on the Centurion, one for terminal I/O, and one for data I/O. The MUX0 port will be connected to the terminal like normal, and MUX1 (the port right under MUX0) will be connected to the host computer.

```
ADDS 200       Centurion     Host Computer
TERMINAL <--> MUX0 | MUX1 <--> SERIALDIR
```

After this systems have been connected, the SerialDir program needs to be setup and ran. This can be most easily done by just downloading and extracting Setup.zip, which contains a up-to-date SerialDir executatble and file system. SerialDir.exe must be run on the command line with the arguments for COM port and baud rate:

```
Usage: SerialDir [Port] [Baudrate]

for example...
./SerialDir COM1 9600
```

### Part 2, Stage 1 Bootstrap

Booting up CCDP happens in 2 stages. The first stage involves using a bootstrap to load a 256 byte bootloader program into memory. This will be done with a short 24 byte TOS type-in program, as there currently are no boot ROMs on the real system for booting serial.

Before serial communication can happen, the MUX1 serial terminal needs to be configured to run at 9600 baud, 8N1. This will be done in TOS for simplicites sake. Once we know how to set up the serial ports for other speeds, this step can be changed to allow for CCDP to run at higher speeds.

By loading 0xC5 into 0xF202, this *should* set the serial port to the correct baud rate.

```
/MF202 C5
```

Start by booting the the Centurion into TOS, then type in the following program.

```
/M018E C0 01 E1 F2 03 D0 02 00 55 2A 81 F2 02 2C 11 FA 81 F2 03 A9 20 30 15 F2
/G018E
```

It is important to place the bootstrap program at 0x018E in memory.

Included below is the listing for bootstrap.asm, this may be easier to read while typing in:

```
1 0000 :                         ; --- BOOTSTRAP ---
1 0000 :                         
1 0000 : C0 01                   	ld		bl,01
1 0002 : E1 F2 03                	st		bl,(0xF203)
1 0005 : D0 02 00                	ld		b,0x0200
1 0008 : 55 2A                   	xfr		b,s
1 000A :                         loop:
1 000A : 81 F2 02                	ld		al,(0xF202)
1 000D : 2C                      	srr		al
1 000E : 11 FA                   	bnl		loop
1 0010 : 81 F2 03                	ld		al,(0xF203)
1 0013 : A9                      	st		al,(b)
1 0014 : 20 30                   	inr		bl
1 0016 : 15 F2                   	bnz		loop
1 0018 :                         	
```

### Part 3, Stage 2 Bootstrap

If all has well in the last part, you should be greeted with the following prompt:

```
/G018E

@
```

This means that the stage 1 bootstrap has successfully completed, and the computer is ready to do stage 2. In this stage, a `.SYS` file will be loaded somewhere into memory, and executed. This should end the bootstrap process, and starting running CCDP. To do this, simply type in the filename for the `.SYS` file you want to boot in upper case, and hit enter. Right now the only bootable file in Setup.zip is `32K.SYS`.

```
@32K.SYS
```

After this is done, the system should fully come up in a few moments.

