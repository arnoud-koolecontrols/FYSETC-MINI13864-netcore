Running armbian on A orange pi Zero plus (Allwinner H5) with the following changes:

sun50i-h5-spi-add-cs1.dts and sun50i-h5-spi-add-cs1.dtbo

	This overlay is used to change the chipselect line to a pin wich is not used on the orange pi zero plus.
	Note it is not yet a custom overlay so you should overwrite the original one. 
	/boot/dtb-5.4.43-sunxi64/allwinner
	Downside is that when the overlays are updated this change gets lost.

spi-double-spidev-cs.dts and spi-double-spidev-cs.dtbo
	
	This overlay is used to create two spi devices on one spidev: spidev1.0 and spidev1.1
	Note this overlay is a custom overlay and should be place in /boot/overlay-user


/boot/armbianEnv.txt

	verbosity=1
	console=serial
	overlay_prefix=sun50i-h5
	overlays=usbhost2 usbhost3 spi-spidev spi-add-cs1
	rootdev=UUID=3cc633ab-6d45-4a9e-89f4-ae2e174d82c6
	rootfstype=ext4
	param_spidev_spi_bus=1
	user_overlays=spi-double-spidev-cs
	usbstoragequirks=0x2537:0x1066:u,0x2537:0x1068:u