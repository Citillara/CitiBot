-- phpMyAdmin SQL Dump
-- version 4.2.12deb2+deb8u2
-- http://www.phpmyadmin.net
--
-- Client :  localhost
-- Généré le :  Dim 25 Mars 2018 à 08:36
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

-- --------------------------------------------------------

--
-- Structure de la table `t_bots`
--

CREATE TABLE IF NOT EXISTS `t_bots` (
`Id` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Password` varchar(100) NOT NULL,
  `Enabled` tinyint(1) unsigned zerofill NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `t_bots_channels`
--

CREATE TABLE IF NOT EXISTS `t_bots_channels` (
`Id` int(11) NOT NULL,
  `Channel` varchar(50) NOT NULL,
  `Greetings` tinyint(4) NOT NULL,
  `BotId` int(11) NOT NULL,
  `AutoJoin` int(11) NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `t_bots_plugins`
--

CREATE TABLE IF NOT EXISTS `t_bots_plugins` (
`Id` int(11) NOT NULL,
  `PluginName` varchar(50) NOT NULL,
  `BotId` int(11) NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `t_cookie_calories`
--

CREATE TABLE IF NOT EXISTS `t_cookie_calories` (
`Id` int(11) NOT NULL,
  `Text` varchar(100) NOT NULL,
  `Calories` int(11) NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=250 DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `t_cookie_channels`
--

CREATE TABLE IF NOT EXISTS `t_cookie_channels` (
`Id` int(11) NOT NULL,
  `Channel` varchar(100) NOT NULL,
  `CookieDelay` int(11) NOT NULL DEFAULT '300',
  `ChangedLast` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE CURRENT_TIMESTAMP,
  `ChangedBy` varchar(100) NOT NULL,
  `BribeDelay` int(11) NOT NULL DEFAULT '600',
  `StealDelay` int(11) NOT NULL DEFAULT '300',
  `CookieCheers` int(11) NOT NULL DEFAULT '0',
  `SubGreetings` varchar(200) NOT NULL DEFAULT '',
  `Status` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `t_cookie_flavours`
--

CREATE TABLE IF NOT EXISTS `t_cookie_flavours` (
`Id` int(11) NOT NULL,
  `Channel` varchar(100) NOT NULL,
  `Status` int(11) NOT NULL DEFAULT '0',
  `Text` varchar(200) NOT NULL,
  `AddedBy` varchar(50) NOT NULL,
  `AddedDate` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB AUTO_INCREMENT=371 DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `t_cookie_poll`
--

CREATE TABLE IF NOT EXISTS `t_cookie_poll` (
`Id` int(11) NOT NULL,
  `CookieChannelId` int(11) NOT NULL DEFAULT '0',
  `Status` int(11) NOT NULL DEFAULT '0',
  `Duration` int(11) NOT NULL DEFAULT '0',
  `Title` varchar(150) NOT NULL DEFAULT '0',
  `StartTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreationTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `t_cookie_poll_options`
--

CREATE TABLE IF NOT EXISTS `t_cookie_poll_options` (
`Id` int(11) NOT NULL,
  `PollId` int(11) NOT NULL DEFAULT '0',
  `Text` varchar(50) NOT NULL DEFAULT '0',
  `Votes` int(11) NOT NULL DEFAULT '0',
  `Order` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `t_cookie_users`
--

CREATE TABLE IF NOT EXISTS `t_cookie_users` (
`Id` int(11) NOT NULL,
  `TwitchUserId` int(11) NOT NULL,
  `Channel` varchar(100) NOT NULL,
  `CookieReceived` int(11) DEFAULT NULL,
  `TopCookieCount` int(11) NOT NULL DEFAULT '0',
  `LastReceived` timestamp NOT NULL DEFAULT '1999-12-31 23:00:00',
  `LastYoshiBribe` timestamp NOT NULL DEFAULT '1999-12-31 23:00:00',
  `LastSend` timestamp NOT NULL DEFAULT '1999-12-31 23:00:00',
  `LastSteal` timestamp NOT NULL DEFAULT '1999-12-31 23:00:00',
  `UpdateTime` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00'
) ENGINE=InnoDB AUTO_INCREMENT=571 DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `t_dog_users`
--

CREATE TABLE IF NOT EXISTS `t_dog_users` (
`Id` int(11) NOT NULL,
  `Username` varchar(100) NOT NULL,
  `LastReceived` timestamp NULL DEFAULT NULL,
  `BonesReceived` int(11) DEFAULT NULL,
  `Channel` varchar(100) NOT NULL,
  `UpdateTime` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE CURRENT_TIMESTAMP,
  `TopBonesCount` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB AUTO_INCREMENT=293 DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `t_poll`
--

CREATE TABLE IF NOT EXISTS `t_poll` (
`Id` int(11) NOT NULL,
  `Title` varchar(500) NOT NULL DEFAULT '0',
  `CreationTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `AllowMultiple` int(11) NOT NULL,
  `AllowAdditions` int(11) NOT NULL,
  `Type` int(11) NOT NULL,
  `Duration` int(11) NOT NULL,
  `Status` int(11) NOT NULL,
  `Owner` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 ROW_FORMAT=COMPACT;

-- --------------------------------------------------------

--
-- Structure de la table `t_poll_options`
--

CREATE TABLE IF NOT EXISTS `t_poll_options` (
`Id` int(11) NOT NULL,
  `PollId` int(11) NOT NULL DEFAULT '0',
  `Text` varchar(50) NOT NULL DEFAULT '0',
  `Votes` int(11) NOT NULL DEFAULT '0',
  `Order` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=latin1 ROW_FORMAT=COMPACT;

-- --------------------------------------------------------

--
-- Structure de la table `t_poll_votes`
--

CREATE TABLE IF NOT EXISTS `t_poll_votes` (
`Id` int(11) NOT NULL,
  `UserId` int(11) NOT NULL DEFAULT '0',
  `OptionId` int(11) NOT NULL DEFAULT '0',
  `Amount` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `t_twitch_users`
--

CREATE TABLE IF NOT EXISTS `t_twitch_users` (
`Id` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `TwitchId` bigint(20) DEFAULT NULL,
  `DisplayName` varchar(50) DEFAULT NULL,
  `FFZId` bigint(20) DEFAULT NULL,
  `LogoUrl` varchar(3000) DEFAULT NULL,
  `TwitchIconId` bigint(20) DEFAULT NULL,
  `ApiToken` varchar(128) DEFAULT NULL,
  `IsAdmin` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB AUTO_INCREMENT=495 DEFAULT CHARSET=latin1;

--
-- Index pour les tables exportées
--

--
-- Index pour la table `t_bots`
--
ALTER TABLE `t_bots`
 ADD PRIMARY KEY (`Id`);

--
-- Index pour la table `t_bots_channels`
--
ALTER TABLE `t_bots_channels`
 ADD PRIMARY KEY (`Id`), ADD KEY `FK_bot_channels_BotId` (`BotId`);

--
-- Index pour la table `t_bots_plugins`
--
ALTER TABLE `t_bots_plugins`
 ADD PRIMARY KEY (`Id`), ADD KEY `fk_botid` (`BotId`);

--
-- Index pour la table `t_cookie_calories`
--
ALTER TABLE `t_cookie_calories`
 ADD PRIMARY KEY (`Id`);

--
-- Index pour la table `t_cookie_channels`
--
ALTER TABLE `t_cookie_channels`
 ADD PRIMARY KEY (`Id`), ADD UNIQUE KEY `Channel` (`Channel`);

--
-- Index pour la table `t_cookie_flavours`
--
ALTER TABLE `t_cookie_flavours`
 ADD PRIMARY KEY (`Id`), ADD KEY `idx_channel` (`Channel`);

--
-- Index pour la table `t_cookie_poll`
--
ALTER TABLE `t_cookie_poll`
 ADD PRIMARY KEY (`Id`), ADD KEY `FK__t_cookie_channels` (`CookieChannelId`), ADD KEY `Status` (`Status`);

--
-- Index pour la table `t_cookie_poll_options`
--
ALTER TABLE `t_cookie_poll_options`
 ADD PRIMARY KEY (`Id`), ADD KEY `FK__t_cookie_poll` (`PollId`);

--
-- Index pour la table `t_cookie_users`
--
ALTER TABLE `t_cookie_users`
 ADD PRIMARY KEY (`Id`), ADD UNIQUE KEY `Channel_TwitchUserId` (`Channel`,`TwitchUserId`), ADD KEY `TwitchUserId` (`TwitchUserId`), ADD KEY `Channel` (`Channel`);

--
-- Index pour la table `t_dog_users`
--
ALTER TABLE `t_dog_users`
 ADD PRIMARY KEY (`Id`), ADD UNIQUE KEY `idx_channel_username` (`Username`,`Channel`), ADD KEY `idx_username` (`Username`), ADD KEY `idx_channel` (`Channel`), ADD KEY `idx_bones_received` (`BonesReceived`);

--
-- Index pour la table `t_poll`
--
ALTER TABLE `t_poll`
 ADD PRIMARY KEY (`Id`), ADD KEY `FK_t_poll_t_twitch_users` (`Owner`);

--
-- Index pour la table `t_poll_options`
--
ALTER TABLE `t_poll_options`
 ADD PRIMARY KEY (`Id`), ADD KEY `FK_t_generic_poll_options_t_generic_poll` (`PollId`);

--
-- Index pour la table `t_poll_votes`
--
ALTER TABLE `t_poll_votes`
 ADD PRIMARY KEY (`Id`), ADD KEY `FK_t_poll_votes_t_poll_options` (`OptionId`), ADD KEY `FK_t_poll_votes_t_twitch_users` (`UserId`);

--
-- Index pour la table `t_twitch_users`
--
ALTER TABLE `t_twitch_users`
 ADD PRIMARY KEY (`Id`), ADD UNIQUE KEY `Name` (`Name`), ADD UNIQUE KEY `TwitchId` (`TwitchId`), ADD FULLTEXT KEY `DisplayName` (`DisplayName`);

--
-- AUTO_INCREMENT pour les tables exportées
--

--
-- AUTO_INCREMENT pour la table `t_bots`
--
ALTER TABLE `t_bots`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=3;
--
-- AUTO_INCREMENT pour la table `t_bots_channels`
--
ALTER TABLE `t_bots_channels`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=10;
--
-- AUTO_INCREMENT pour la table `t_bots_plugins`
--
ALTER TABLE `t_bots_plugins`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=4;
--
-- AUTO_INCREMENT pour la table `t_cookie_calories`
--
ALTER TABLE `t_cookie_calories`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=250;
--
-- AUTO_INCREMENT pour la table `t_cookie_channels`
--
ALTER TABLE `t_cookie_channels`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=13;
--
-- AUTO_INCREMENT pour la table `t_cookie_flavours`
--
ALTER TABLE `t_cookie_flavours`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=371;
--
-- AUTO_INCREMENT pour la table `t_cookie_poll`
--
ALTER TABLE `t_cookie_poll`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT pour la table `t_cookie_poll_options`
--
ALTER TABLE `t_cookie_poll_options`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT pour la table `t_cookie_users`
--
ALTER TABLE `t_cookie_users`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=571;
--
-- AUTO_INCREMENT pour la table `t_dog_users`
--
ALTER TABLE `t_dog_users`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=293;
--
-- AUTO_INCREMENT pour la table `t_poll`
--
ALTER TABLE `t_poll`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT pour la table `t_poll_options`
--
ALTER TABLE `t_poll_options`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT pour la table `t_poll_votes`
--
ALTER TABLE `t_poll_votes`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT pour la table `t_twitch_users`
--
ALTER TABLE `t_twitch_users`
MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=495;
--
-- Contraintes pour les tables exportées
--

--
-- Contraintes pour la table `t_bots_channels`
--
ALTER TABLE `t_bots_channels`
ADD CONSTRAINT `FK_t_bots_channels_t_bots` FOREIGN KEY (`BotId`) REFERENCES `t_bots` (`Id`);

--
-- Contraintes pour la table `t_bots_plugins`
--
ALTER TABLE `t_bots_plugins`
ADD CONSTRAINT `FK_t_bots_plugins_t_bots` FOREIGN KEY (`BotId`) REFERENCES `t_bots` (`Id`);

--
-- Contraintes pour la table `t_cookie_poll`
--
ALTER TABLE `t_cookie_poll`
ADD CONSTRAINT `FK__t_cookie_channels` FOREIGN KEY (`CookieChannelId`) REFERENCES `t_cookie_channels` (`Id`);

--
-- Contraintes pour la table `t_cookie_poll_options`
--
ALTER TABLE `t_cookie_poll_options`
ADD CONSTRAINT `FK__t_cookie_poll` FOREIGN KEY (`PollId`) REFERENCES `t_cookie_poll` (`Id`);

--
-- Contraintes pour la table `t_cookie_users`
--
ALTER TABLE `t_cookie_users`
ADD CONSTRAINT `FK_t_cookie_users_t_twitch_users` FOREIGN KEY (`TwitchUserId`) REFERENCES `t_twitch_users` (`Id`);

--
-- Contraintes pour la table `t_poll`
--
ALTER TABLE `t_poll`
ADD CONSTRAINT `FK_t_poll_t_twitch_users` FOREIGN KEY (`Owner`) REFERENCES `t_twitch_users` (`Id`);

--
-- Contraintes pour la table `t_poll_options`
--
ALTER TABLE `t_poll_options`
ADD CONSTRAINT `FK_t_generic_poll_options_t_generic_poll` FOREIGN KEY (`PollId`) REFERENCES `t_poll` (`Id`);

--
-- Contraintes pour la table `t_poll_votes`
--
ALTER TABLE `t_poll_votes`
ADD CONSTRAINT `FK_t_poll_votes_t_poll_options` FOREIGN KEY (`OptionId`) REFERENCES `t_poll_options` (`Id`),
ADD CONSTRAINT `FK_t_poll_votes_t_twitch_users` FOREIGN KEY (`UserId`) REFERENCES `t_twitch_users` (`Id`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
