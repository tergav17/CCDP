aswrx6 -l listing_ccdp.txt ccdp.asm
ldwrx6 -b -C 28672 ccdp.o -o ccdp.raw
dd if=ccdp.raw bs=28672 skip=1 of=bin/CCDP.BIN
cat headers/32K.header bin/CCDP.BIN > bin/32K.SYS

ldwrx6 -b -C 15360 ccdp.o -o ccdp.raw
dd if=ccdp.raw bs=15360 skip=1 of=bin/CCDP.BIN
cat headers/16K.header bin/CCDP.BIN > bin/16K.SYS
rm ccdp.o ccdp.raw bin/CCDP.BIN

aswrx6 -l listing_hello.txt hello.asm
ldwrx6 -b -C 512 hello.o -o hello.raw
dd if=hello.raw bs=512 skip=1 of=bin/HELLO.BIN
rm hello.raw hello.o

aswrx6 -l listing_dir.txt dir.asm
ldwrx6 -b -C 512 dir.o -o dir.raw
dd if=dir.raw bs=512 skip=1 of=bin/DIR.BIN
rm dir.raw dir.o

aswrx6 -l listing_type.txt type.asm
ldwrx6 -b -C 512 type.o -o type.raw
dd if=type.raw bs=512 skip=1 of=bin/TYPE.BIN
rm type.raw type.o

aswrx6 -l listing_copy.txt copy.asm
ldwrx6 -b -C 512 copy.o -o copy.raw
dd if=copy.raw bs=512 skip=1 of=bin/COPY.BIN
rm copy.raw copy.o

aswrx6 -l listing_delete.txt delete.asm
ldwrx6 -b -C 512 delete.o -o delete.raw
dd if=delete.raw bs=512 skip=1 of=bin/DELETE.BIN
rm delete.raw delete.o

aswrx6 -l listing_boot.txt boot.asm
ldwrx6 -b -C 512 boot.o -o boot.raw
dd if=boot.raw bs=512 skip=1 of=bin/BOOT.BIN
rm boot.raw boot.o

aswrx6 -l listing_bootstrap.txt bootstrap.asm
rm bootstrap.o

mv *.txt listings/