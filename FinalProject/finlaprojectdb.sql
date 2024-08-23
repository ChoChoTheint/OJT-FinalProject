/*
SQLyog Community v13.2.1 (64 bit)
MySQL - 8.4.0 : Database - finalprojectdb
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`finalprojectdb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `finalprojectdb`;

/*Table structure for table `event_log` */

DROP TABLE IF EXISTS `event_log`;

CREATE TABLE `event_log` (
  `log_type` int NOT NULL COMMENT '0 = info, 1 = Warning, 2 = Error, 3 = Insert, 4 = Update, 5 = Delete',
  `log_datetime` datetime NOT NULL,
  `log_message` text,
  `error_message` text,
  `form_name` varchar(500) NOT NULL COMMENT 'function name',
  `source` varchar(500) NOT NULL COMMENT 'controller name'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Table structure for table `exercise` */

DROP TABLE IF EXISTS `exercise`;

CREATE TABLE `exercise` (
  `exercise_id` int NOT NULL AUTO_INCREMENT,
  `exercise_no` varchar(500) COLLATE utf8mb4_general_ci NOT NULL,
  `description` varchar(500) COLLATE utf8mb4_general_ci NOT NULL,
  `exercise_content` varchar(500) COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`exercise_id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

/*Table structure for table `exercise_assign` */

DROP TABLE IF EXISTS `exercise_assign`;

CREATE TABLE `exercise_assign` (
  `exercise_assign_id` int NOT NULL AUTO_INCREMENT,
  `exercise_id` int NOT NULL,
  `user_id` int NOT NULL,
  `mark` int NOT NULL,
  PRIMARY KEY (`exercise_assign_id`),
  KEY `exerciseid_frk` (`exercise_id`),
  KEY `userid_frk` (`user_id`),
  CONSTRAINT `exerciseid_frk` FOREIGN KEY (`exercise_id`) REFERENCES `exercise` (`exercise_id`) ON DELETE CASCADE,
  CONSTRAINT `userid_frk` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Table structure for table `user` */

DROP TABLE IF EXISTS `user`;

CREATE TABLE `user` (
  `user_id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(500) NOT NULL,
  `password` varchar(500) NOT NULL,
  `salt` varchar(500) NOT NULL,
  `login_fail_count` int NOT NULL DEFAULT '0',
  `is_lock` tinyint(1) NOT NULL DEFAULT '0',
  `userlevel_id` int NOT NULL,
  PRIMARY KEY (`user_id`),
  KEY `userlevelid_frk` (`userlevel_id`),
  CONSTRAINT `userlevelid_frk` FOREIGN KEY (`userlevel_id`) REFERENCES `user_level` (`userlevel_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Table structure for table `user_level` */

DROP TABLE IF EXISTS `user_level`;

CREATE TABLE `user_level` (
  `userlevel_id` int NOT NULL AUTO_INCREMENT,
  `userlevel_name` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`userlevel_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Table structure for table `user_level_menu` */

DROP TABLE IF EXISTS `user_level_menu`;

CREATE TABLE `user_level_menu` (
  `userlevel_id` int DEFAULT NULL,
  `endpoint` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
