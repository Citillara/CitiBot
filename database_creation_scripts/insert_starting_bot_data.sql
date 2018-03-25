-- phpMyAdmin SQL Dump
-- version 4.2.12deb2+deb8u2
-- http://www.phpmyadmin.net
--
-- Client :  localhost
-- Généré le :  Dim 25 Mars 2018 à 08:41
-- Version du serveur :  10.0.32-MariaDB-0+deb8u1
-- Version de PHP :  5.6.33-0+deb8u1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Base de données :  `citibot`
--

--
-- Contenu de la table `t_bots`
--

INSERT INTO `t_bots` (`Id`, `Name`, `Password`, `Enabled`) VALUES
(1, 'CitiBot', 'oauth:aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa', 1),
(2, '360Dog', 'oauth:aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa', 0);

--
-- Contenu de la table `t_bots_channels`
--

INSERT INTO `t_bots_channels` (`Id`, `Channel`, `Greetings`, `BotId`, `AutoJoin`) VALUES
(1, '#citillara', 0, 1, 1),
(2, '#citillara', 0, 2, 0),


--
-- Contenu de la table `t_bots_plugins`
--

INSERT INTO `t_bots_plugins` (`Id`, `PluginName`, `BotId`) VALUES
(1, 'CookieGiver', 1),
(2, 'GenericCommands', 1),
(3, 'Dog', 2);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
