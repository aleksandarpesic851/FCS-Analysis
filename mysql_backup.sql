/*
SQLyog Ultimate
MySQL - 10.1.38-MariaDB : Database - fcs_analysis
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`fcs_analysis` /*!40100 DEFAULT CHARACTER SET latin1 */;

/*Table structure for table `fcs` */

DROP TABLE IF EXISTS `fcs`;

CREATE TABLE `fcs` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `createdAt` datetime NOT NULL,
  `updatedAt` datetime NOT NULL,
  `enabled` bit(1) NOT NULL DEFAULT b'1',
  `fcs_name` varchar(255) NOT NULL,
  `fcs_file_name` varchar(255) NOT NULL,
  `fcs_path` varchar(255) NOT NULL,
  `user_id` int(11) NOT NULL,
  `fcs_type` blob NOT NULL,
  `wbc_3cells` varchar(255) DEFAULT NULL,
  `wbc_gate2` varchar(255) DEFAULT NULL,
  `wbc_heatmap` varchar(255) DEFAULT NULL,
  `nomenclature` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=132 DEFAULT CHARSET=latin1;

/*Data for the table `fcs` */

insert  into `fcs`(`id`,`createdAt`,`updatedAt`,`enabled`,`fcs_name`,`fcs_file_name`,`fcs_path`,`user_id`,`fcs_type`,`wbc_3cells`,`wbc_gate2`,`wbc_heatmap`,`nomenclature`) values 
(112,'2020-03-09 09:01:12','2020-03-09 09:01:12','\0','H49520_CD16APC_25mW_01.fcs','uwtmqme1.huh_H49520_CD16APC_25mW_01.fcs','/uploads/fcs/wbc/fcs/uwtmqme1.huh_H49520_CD16APC_25mW_01.fcs',1,'wbc','/uploads/fcs/wbc/3cell/uwtmqme1.huh_H49520_CD16APC_25mW_01.fcs.obj','/uploads/fcs/wbc/gate2/uwtmqme1.huh_H49520_CD16APC_25mW_01.fcs.obj','/uploads/fcs/wbc/heatmap/uwtmqme1.huh_H49520_CD16APC_25mW_01.fcs.png',0),
(113,'2020-03-09 09:01:17','2020-03-09 09:01:17','\0','H49819_CD16APC_25mW_01.fcs','llbqsy41.zut_H49819_CD16APC_25mW_01.fcs','/uploads/fcs/wbc/fcs/llbqsy41.zut_H49819_CD16APC_25mW_01.fcs',1,'wbc','/uploads/fcs/wbc/3cell/llbqsy41.zut_H49819_CD16APC_25mW_01.fcs.obj','/uploads/fcs/wbc/gate2/llbqsy41.zut_H49819_CD16APC_25mW_01.fcs.obj','/uploads/fcs/wbc/heatmap/llbqsy41.zut_H49819_CD16APC_25mW_01.fcs.png',0),
(114,'2020-03-09 09:01:23','2020-03-09 09:01:23','\0','H57070_BASO_25mW_01.fcs','chxherjt.sn2_H57070_BASO_25mW_01.fcs','/uploads/fcs/wbc/fcs/chxherjt.sn2_H57070_BASO_25mW_01.fcs',1,'wbc','/uploads/fcs/wbc/3cell/chxherjt.sn2_H57070_BASO_25mW_01.fcs.obj','/uploads/fcs/wbc/gate2/chxherjt.sn2_H57070_BASO_25mW_01.fcs.obj','/uploads/fcs/wbc/heatmap/chxherjt.sn2_H57070_BASO_25mW_01.fcs.png',0),
(115,'2020-03-10 08:11:30','2020-03-10 08:11:30','\0','K18079_CD16APC_25mW_01.fcs','knvvqu31.sap_K18079_CD16APC_25mW_01.fcs','/uploads/fcs/wbc/fcs/knvvqu31.sap_K18079_CD16APC_25mW_01.fcs',1,'wbc_eos','/uploads/fcs/wbc/3cell/knvvqu31.sap_K18079_CD16APC_25mW_01.fcs.obj','/uploads/fcs/wbc/gate2/knvvqu31.sap_K18079_CD16APC_25mW_01.fcs.obj','/uploads/fcs/wbc/heatmap/knvvqu31.sap_K18079_CD16APC_25mW_01.fcs.png',0),
(116,'2020-03-10 09:21:20','2020-03-10 09:21:20','\0','RBC sample.fcs','dzjavoyn.ncm_RBC sample.fcs','/uploads/fcs/rbc/fcs/dzjavoyn.ncm_RBC sample.fcs',1,'rbc',NULL,NULL,NULL,0),
(117,'2020-03-10 09:34:49','2020-03-10 09:34:49','','RBC1_1.fcs','01ayhb03.llt_RBC1_1.fcs','/uploads/fcs/rbc/fcs/01ayhb03.llt_RBC1_1.fcs',1,'rbc',NULL,NULL,NULL,2),
(118,'2020-03-11 05:23:14','2020-03-11 05:23:14','\0','M9504_1_40_30W_01.fcs','0kuxwcey.5m3_M9504_1_40_30W_01.fcs','/uploads/fcs/wbc/fcs/0kuxwcey.5m3_M9504_1_40_30W_01.fcs',1,'wbc_eos','/uploads/fcs/wbc/3cell/0kuxwcey.5m3_M9504_1_40_30W_01.fcs.obj','/uploads/fcs/wbc/gate2/0kuxwcey.5m3_M9504_1_40_30W_01.fcs.obj','/uploads/fcs/wbc/heatmap/0kuxwcey.5m3_M9504_1_40_30W_01.fcs.png',0),
(119,'2020-03-11 05:23:16','2020-03-11 05:23:16','\0','M9504_1_40_30W_06.fcs','wb5uui4b.bim_M9504_1_40_30W_06.fcs','/uploads/fcs/wbc/fcs/wb5uui4b.bim_M9504_1_40_30W_06.fcs',1,'wbc_eos','/uploads/fcs/wbc/3cell/wb5uui4b.bim_M9504_1_40_30W_06.fcs.obj','/uploads/fcs/wbc/gate2/wb5uui4b.bim_M9504_1_40_30W_06.fcs.obj','/uploads/fcs/wbc/heatmap/wb5uui4b.bim_M9504_1_40_30W_06.fcs.png',0),
(120,'2020-03-11 05:47:30','2020-03-11 05:47:30','','M9504_1_40_30W_01.fcs','s1o4jvnc.tou_M9504_1_40_30W_01.fcs','/uploads/fcs/wbc/fcs/s1o4jvnc.tou_M9504_1_40_30W_01.fcs',1,'wbc','/uploads/fcs/wbc/3cell/s1o4jvnc.tou_M9504_1_40_30W_01.fcs.obj','/uploads/fcs/wbc/gate2/s1o4jvnc.tou_M9504_1_40_30W_01.fcs.obj','/uploads/fcs/wbc/heatmap/s1o4jvnc.tou_M9504_1_40_30W_01.fcs.png',0),
(121,'2020-03-11 05:47:31','2020-03-11 05:47:31','\0','M9504_1_40_30W_06.fcs','qjldve5v.gcg_M9504_1_40_30W_06.fcs','/uploads/fcs/wbc/fcs/qjldve5v.gcg_M9504_1_40_30W_06.fcs',1,'wbc','/uploads/fcs/wbc/3cell/qjldve5v.gcg_M9504_1_40_30W_06.fcs.obj','/uploads/fcs/wbc/gate2/qjldve5v.gcg_M9504_1_40_30W_06.fcs.obj','/uploads/fcs/wbc/heatmap/qjldve5v.gcg_M9504_1_40_30W_06.fcs.png',0),
(122,'2020-03-11 06:42:40','2020-03-11 06:42:40','','H49520_CD16APC_25mW_01.fcs','tdf3suqm.asw_H49520_CD16APC_25mW_01.fcs','/uploads/fcs/wbc/fcs/tdf3suqm.asw_H49520_CD16APC_25mW_01.fcs',1,'wbc_eos','/uploads/fcs/wbc/3cell/tdf3suqm.asw_H49520_CD16APC_25mW_01.fcs.obj','/uploads/fcs/wbc/gate2/tdf3suqm.asw_H49520_CD16APC_25mW_01.fcs.obj','/uploads/fcs/wbc/heatmap/tdf3suqm.asw_H49520_CD16APC_25mW_01.fcs.png',0),
(123,'2020-03-11 06:42:45','2020-03-11 06:42:45','','H49819_CD16APC_25mW_01.fcs','ezbw5p0s.rc0_H49819_CD16APC_25mW_01.fcs','/uploads/fcs/wbc/fcs/ezbw5p0s.rc0_H49819_CD16APC_25mW_01.fcs',1,'wbc_eos','/uploads/fcs/wbc/3cell/ezbw5p0s.rc0_H49819_CD16APC_25mW_01.fcs.obj','/uploads/fcs/wbc/gate2/ezbw5p0s.rc0_H49819_CD16APC_25mW_01.fcs.obj','/uploads/fcs/wbc/heatmap/ezbw5p0s.rc0_H49819_CD16APC_25mW_01.fcs.png',0),
(124,'2020-03-11 06:42:54','2020-03-11 06:42:54','','S65506_CD16APC_25mW_01.fcs','ou5beooa.co3_S65506_CD16APC_25mW_01.fcs','/uploads/fcs/wbc/fcs/ou5beooa.co3_S65506_CD16APC_25mW_01.fcs',1,'wbc_eos','/uploads/fcs/wbc/3cell/ou5beooa.co3_S65506_CD16APC_25mW_01.fcs.obj','/uploads/fcs/wbc/gate2/ou5beooa.co3_S65506_CD16APC_25mW_01.fcs.obj','/uploads/fcs/wbc/heatmap/ou5beooa.co3_S65506_CD16APC_25mW_01.fcs.png',0),
(125,'2020-03-11 06:43:05','2020-03-11 06:43:05','','S65683_CD16APC_25mW_01.fcs','md3phxgq.w5l_S65683_CD16APC_25mW_01.fcs','/uploads/fcs/wbc/fcs/md3phxgq.w5l_S65683_CD16APC_25mW_01.fcs',1,'wbc_eos','/uploads/fcs/wbc/3cell/md3phxgq.w5l_S65683_CD16APC_25mW_01.fcs.obj','/uploads/fcs/wbc/gate2/md3phxgq.w5l_S65683_CD16APC_25mW_01.fcs.obj','/uploads/fcs/wbc/heatmap/md3phxgq.w5l_S65683_CD16APC_25mW_01.fcs.png',0),
(126,'2020-03-12 09:42:49','2020-03-12 09:42:49','','0002.FCS','sauq4dqf.li5_0002.FCS','/uploads/fcs/wbc/fcs/sauq4dqf.li5_0002.FCS',1,'wbc_aml','/uploads/fcs/wbc/3cell/sauq4dqf.li5_0002.FCS.obj','','/uploads/fcs/wbc/heatmap/sauq4dqf.li5_0002.FCS.png',3),
(127,'2020-03-12 09:42:49','2020-03-12 09:42:49','','0001 (1).FCS','hzws4ibh.lon_0001 (1).FCS','/uploads/fcs/wbc/fcs/hzws4ibh.lon_0001 (1).FCS',1,'wbc_aml','/uploads/fcs/wbc/3cell/hzws4ibh.lon_0001 (1).FCS.obj','','/uploads/fcs/wbc/heatmap/hzws4ibh.lon_0001 (1).FCS.png',3),
(128,'2020-03-12 09:42:54','2020-03-12 09:42:54','','0033 (1).FCS','pvmkb1ys.pfj_0033 (1).FCS','/uploads/fcs/wbc/fcs/pvmkb1ys.pfj_0033 (1).FCS',1,'wbc_aml','/uploads/fcs/wbc/3cell/pvmkb1ys.pfj_0033 (1).FCS.obj','','/uploads/fcs/wbc/heatmap/pvmkb1ys.pfj_0033 (1).FCS.png',3),
(129,'2020-03-12 09:42:55','2020-03-12 09:42:55','','0034.FCS','302lvqem.x5l_0034.FCS','/uploads/fcs/wbc/fcs/302lvqem.x5l_0034.FCS',1,'wbc_aml','/uploads/fcs/wbc/3cell/302lvqem.x5l_0034.FCS.obj','','/uploads/fcs/wbc/heatmap/302lvqem.x5l_0034.FCS.png',3),
(130,'2020-03-12 09:42:57','2020-03-12 09:42:57','','2784.FCS','di5u1k43.ejq_2784.FCS','/uploads/fcs/wbc/fcs/di5u1k43.ejq_2784.FCS',1,'wbc_aml','/uploads/fcs/wbc/3cell/di5u1k43.ejq_2784.FCS.obj','','/uploads/fcs/wbc/heatmap/di5u1k43.ejq_2784.FCS.png',3),
(131,'2020-03-12 09:42:58','2020-03-12 09:42:58','','2734.FCS','pwrbyijv.ika_2734.FCS','/uploads/fcs/wbc/fcs/pwrbyijv.ika_2734.FCS',1,'wbc_aml','/uploads/fcs/wbc/3cell/pwrbyijv.ika_2734.FCS.obj','','/uploads/fcs/wbc/heatmap/pwrbyijv.ika_2734.FCS.png',3);

/*Table structure for table `users` */

DROP TABLE IF EXISTS `users`;

CREATE TABLE `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `createdAt` datetime NOT NULL,
  `updatedAt` datetime NOT NULL,
  `enabled` bit(1) NOT NULL DEFAULT b'1',
  `user_email` varchar(255) NOT NULL,
  `user_password` varchar(255) NOT NULL,
  `user_name` varchar(255) NOT NULL,
  `user_avatar` text,
  `user_role` varchar(255) NOT NULL,
  `user_phone` varchar(255) DEFAULT NULL,
  `user_address` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

/*Data for the table `users` */

insert  into `users`(`id`,`createdAt`,`updatedAt`,`enabled`,`user_email`,`user_password`,`user_name`,`user_avatar`,`user_role`,`user_phone`,`user_address`) values 
(1,'2020-02-28 23:48:08','2020-02-28 23:48:08','','admin@gmail.com','secret','Admin','/uploads/avatars/admin.png','Admin',NULL,NULL);

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
