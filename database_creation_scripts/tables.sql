-- phpMyAdmin SQL Dump
-- version 5.0.4deb2+deb11u1
-- https://www.phpmyadmin.net/
--
-- Hôte : localhost:3306
-- Généré le : ven. 22 juil. 2022 à 18:18
-- Version du serveur :  10.5.15-MariaDB-0+deb11u1
-- Version de PHP : 7.4.28

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de données : `citibot`
--

-- --------------------------------------------------------

--
-- Structure de la table `t_bots`
--

CREATE TABLE `t_bots` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Password` varchar(100) NOT NULL,
  `Enabled` tinyint(1) UNSIGNED ZEROFILL NOT NULL,
  `CallbackPort` smallint(5) UNSIGNED NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `t_bots_channels`
--

CREATE TABLE `t_bots_channels` (
  `Id` int(11) NOT NULL,
  `Channel` varchar(50) NOT NULL,
  `Greetings` tinyint(4) NOT NULL,
  `BotId` int(11) NOT NULL,
  `AutoJoin` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `t_bots_plugins`
--

CREATE TABLE `t_bots_plugins` (
  `Id` int(11) NOT NULL,
  `PluginName` varchar(50) NOT NULL,
  `BotId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `t_cookie_calories`
--

CREATE TABLE `t_cookie_calories` (
  `Id` int(11) NOT NULL,
  `Text` varchar(100) NOT NULL,
  `Calories` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `t_cookie_channels`
--

CREATE TABLE `t_cookie_channels` (
  `Id` int(11) NOT NULL,
  `Channel` varchar(100) NOT NULL,
  `CookieDelay` int(11) NOT NULL DEFAULT 300,
  `ChangedLast` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE current_timestamp(),
  `ChangedBy` varchar(100) NOT NULL,
  `BribeDelay` int(11) NOT NULL DEFAULT 600,
  `StealDelay` int(11) NOT NULL DEFAULT 300,
  `CookieCheers` int(11) NOT NULL DEFAULT 0,
  `SubGreetings` varchar(200) DEFAULT '',
  `Status` int(11) NOT NULL DEFAULT 0,
  `CustomCookieEmote` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `t_cookie_flavours`
--

CREATE TABLE `t_cookie_flavours` (
  `Id` int(11) NOT NULL,
  `Channel` varchar(100) NOT NULL,
  `Status` int(11) NOT NULL DEFAULT 0,
  `Text` varchar(200) NOT NULL,
  `AddedBy` varchar(50) NOT NULL,
  `AddedDate` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE current_timestamp(),
  `Test` text CHARACTER SET utf32 COLLATE utf32_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `t_cookie_poll`
--

CREATE TABLE `t_cookie_poll` (
  `Id` int(11) NOT NULL,
  `CookieChannelId` int(11) NOT NULL DEFAULT 0,
  `Status` int(11) NOT NULL DEFAULT 0,
  `Duration` int(11) NOT NULL DEFAULT 0,
  `Title` varchar(150) NOT NULL DEFAULT '0',
  `StartTime` timestamp NOT NULL DEFAULT current_timestamp(),
  `CreationTime` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `t_cookie_poll_options`
--

CREATE TABLE `t_cookie_poll_options` (
  `Id` int(11) NOT NULL,
  `PollId` int(11) NOT NULL DEFAULT 0,
  `Text` varchar(50) NOT NULL DEFAULT '0',
  `Votes` int(11) NOT NULL DEFAULT 0,
  `Order` int(11) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `t_cookie_users`
--

CREATE TABLE `t_cookie_users` (
  `Id` int(11) NOT NULL,
  `TwitchUserId` int(11) NOT NULL,
  `Channel` varchar(100) NOT NULL,
  `CookieReceived` int(11) DEFAULT NULL,
  `TopCookieCount` int(11) NOT NULL DEFAULT 0,
  `LastReceived` timestamp NOT NULL DEFAULT '1999-12-31 23:00:00',
  `LastYoshiBribe` timestamp NOT NULL DEFAULT '1999-12-31 23:00:00',
  `LastSend` timestamp NOT NULL DEFAULT '1999-12-31 23:00:00',
  `LastSteal` timestamp NOT NULL DEFAULT '1999-12-31 23:00:00',
  `UpdateTime` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `CookiesSent` int(11) NOT NULL DEFAULT 0,
  `CookiesLostToYoshi` int(11) NOT NULL DEFAULT 0,
  `CookiesReceivedByOthers` int(11) NOT NULL DEFAULT 0,
  `CookiesGenerated` int(11) NOT NULL DEFAULT 0,
  `CookiesGivenToYoshi` int(11) NOT NULL DEFAULT 0,
  `CookiesDestroyedByYoshi` int(11) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `t_counters`
--

CREATE TABLE `t_counters` (
  `Id` int(11) NOT NULL,
  `Channel` varchar(50) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Count` bigint(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `t_logs`
--

CREATE TABLE `t_logs` (
  `Id` int(11) NOT NULL,
  `LogDate` datetime NOT NULL,
  `Type` int(11) NOT NULL,
  `Level` int(11) NOT NULL,
  `Criteria1` varchar(200) NOT NULL,
  `Criteria2` varchar(200) DEFAULT NULL,
  `Data` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `t_moderation_blacklist_item`
--

CREATE TABLE `t_moderation_blacklist_item` (
  `Id` int(11) NOT NULL,
  `Channel` varchar(100) NOT NULL,
  `Status` int(11) NOT NULL DEFAULT 0,
  `Text` varchar(200) NOT NULL,
  `AddedBy` varchar(50) NOT NULL,
  `AddedDate` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- --------------------------------------------------------

--
-- Structure de la table `t_poll`
--

CREATE TABLE `t_poll` (
  `Id` int(11) NOT NULL,
  `Title` varchar(500) NOT NULL DEFAULT '0',
  `CreationTime` timestamp NOT NULL DEFAULT current_timestamp(),
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

CREATE TABLE `t_poll_options` (
  `Id` int(11) NOT NULL,
  `PollId` int(11) NOT NULL DEFAULT 0,
  `Text` varchar(50) NOT NULL DEFAULT '0',
  `Votes` int(11) NOT NULL DEFAULT 0,
  `Order` int(11) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=latin1 ROW_FORMAT=COMPACT;

-- --------------------------------------------------------

--
-- Structure de la table `t_poll_votes`
--

CREATE TABLE `t_poll_votes` (
  `Id` int(11) NOT NULL,
  `UserId` int(11) NOT NULL DEFAULT 0,
  `OptionId` int(11) NOT NULL DEFAULT 0,
  `Amount` int(11) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `t_settings`
--

CREATE TABLE `t_settings` (
  `Name` varchar(200) NOT NULL,
  `Value` varchar(400) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `t_twitch_users`
--

CREATE TABLE `t_twitch_users` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `TwitchId` bigint(20) DEFAULT NULL,
  `DisplayName` varchar(50) DEFAULT NULL,
  `FFZId` bigint(20) DEFAULT NULL,
  `LogoUrl` varchar(3000) DEFAULT NULL,
  `TwitchIconId` bigint(20) DEFAULT NULL,
  `ApiToken` varchar(128) DEFAULT NULL,
  `IsAdmin` int(11) NOT NULL DEFAULT 0,
  `ApiTokenGenerationDate` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Doublure de structure pour la vue `v_cookie_users`
-- (Voir ci-dessous la vue réelle)
--
CREATE TABLE `v_cookie_users` (
`Id` int(11)
,`Username` varchar(50)
,`CookieReceived` int(11)
,`TopCookieCount` int(11)
,`Channel` varchar(100)
,`DisplayName` varchar(50)
,`FFZId` bigint(20)
,`TwitchIconId` bigint(20)
,`LogoUrl` varchar(3000)
,`CookiesSent` int(11)
,`CookiesLostToYoshi` int(11)
,`CookiesReceivedByOthers` int(11)
,`CookiesGenerated` int(11)
,`CookiesGivenToYoshi` int(11)
,`CookiesDestroyedByYoshi` int(11)
);

-- --------------------------------------------------------

--
-- Structure de la vue `v_cookie_users`
--
DROP TABLE IF EXISTS `v_cookie_users`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_cookie_users`  AS SELECT `cu`.`Id` AS `Id`, `tw`.`Name` AS `Username`, `cu`.`CookieReceived` AS `CookieReceived`, `cu`.`TopCookieCount` AS `TopCookieCount`, `cu`.`Channel` AS `Channel`, `tw`.`DisplayName` AS `DisplayName`, `tw`.`FFZId` AS `FFZId`, `tw`.`TwitchIconId` AS `TwitchIconId`, `tw`.`LogoUrl` AS `LogoUrl`, `cu`.`CookiesSent` AS `CookiesSent`, `cu`.`CookiesLostToYoshi` AS `CookiesLostToYoshi`, `cu`.`CookiesReceivedByOthers` AS `CookiesReceivedByOthers`, `cu`.`CookiesGenerated` AS `CookiesGenerated`, `cu`.`CookiesGivenToYoshi` AS `CookiesGivenToYoshi`, `cu`.`CookiesDestroyedByYoshi` AS `CookiesDestroyedByYoshi` FROM (`t_cookie_users` `cu` left join `t_twitch_users` `tw` on(`tw`.`Id` = `cu`.`TwitchUserId`)) ;

--
-- Index pour les tables déchargées
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
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK_bot_channels_BotId` (`BotId`);

--
-- Index pour la table `t_bots_plugins`
--
ALTER TABLE `t_bots_plugins`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_botid` (`BotId`);

--
-- Index pour la table `t_cookie_calories`
--
ALTER TABLE `t_cookie_calories`
  ADD PRIMARY KEY (`Id`);

--
-- Index pour la table `t_cookie_channels`
--
ALTER TABLE `t_cookie_channels`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Channel` (`Channel`);

--
-- Index pour la table `t_cookie_flavours`
--
ALTER TABLE `t_cookie_flavours`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `idx_channel` (`Channel`);

--
-- Index pour la table `t_cookie_poll`
--
ALTER TABLE `t_cookie_poll`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK__t_cookie_channels` (`CookieChannelId`),
  ADD KEY `Status` (`Status`);

--
-- Index pour la table `t_cookie_poll_options`
--
ALTER TABLE `t_cookie_poll_options`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK__t_cookie_poll` (`PollId`);

--
-- Index pour la table `t_cookie_users`
--
ALTER TABLE `t_cookie_users`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Channel_TwitchUserId` (`Channel`,`TwitchUserId`),
  ADD KEY `TwitchUserId` (`TwitchUserId`),
  ADD KEY `Channel` (`Channel`);

--
-- Index pour la table `t_counters`
--
ALTER TABLE `t_counters`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `ChannelId_Name` (`Channel`,`Name`);

--
-- Index pour la table `t_logs`
--
ALTER TABLE `t_logs`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `logs_date_index` (`LogDate`),
  ADD KEY `logs_type` (`Type`),
  ADD KEY `logs_criteria1` (`Criteria1`),
  ADD KEY `logs_criteria2` (`Criteria2`),
  ADD KEY `logs_level` (`Level`),
  ADD KEY `logs_type1criteria` (`Type`,`Criteria1`,`Level`) USING BTREE,
  ADD KEY `logs_type2criterias` (`Type`,`Criteria1`,`Criteria2`,`Level`) USING BTREE;

--
-- Index pour la table `t_moderation_blacklist_item`
--
ALTER TABLE `t_moderation_blacklist_item`
  ADD PRIMARY KEY (`Id`) USING BTREE,
  ADD KEY `idx_channel` (`Channel`) USING BTREE;

--
-- Index pour la table `t_poll`
--
ALTER TABLE `t_poll`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK_t_poll_t_twitch_users` (`Owner`);

--
-- Index pour la table `t_poll_options`
--
ALTER TABLE `t_poll_options`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK_t_generic_poll_options_t_generic_poll` (`PollId`);

--
-- Index pour la table `t_poll_votes`
--
ALTER TABLE `t_poll_votes`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK_t_poll_votes_t_poll_options` (`OptionId`),
  ADD KEY `FK_t_poll_votes_t_twitch_users` (`UserId`);

--
-- Index pour la table `t_settings`
--
ALTER TABLE `t_settings`
  ADD PRIMARY KEY (`Name`);

--
-- Index pour la table `t_twitch_users`
--
ALTER TABLE `t_twitch_users`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Name` (`Name`),
  ADD UNIQUE KEY `TwitchId` (`TwitchId`);
ALTER TABLE `t_twitch_users` ADD FULLTEXT KEY `DisplayName` (`DisplayName`);

--
-- AUTO_INCREMENT pour les tables déchargées
--

--
-- AUTO_INCREMENT pour la table `t_bots`
--
ALTER TABLE `t_bots`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `t_bots_channels`
--
ALTER TABLE `t_bots_channels`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `t_bots_plugins`
--
ALTER TABLE `t_bots_plugins`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `t_cookie_calories`
--
ALTER TABLE `t_cookie_calories`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `t_cookie_channels`
--
ALTER TABLE `t_cookie_channels`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `t_cookie_flavours`
--
ALTER TABLE `t_cookie_flavours`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

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
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `t_counters`
--
ALTER TABLE `t_counters`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `t_logs`
--
ALTER TABLE `t_logs`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `t_moderation_blacklist_item`
--
ALTER TABLE `t_moderation_blacklist_item`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

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
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- Contraintes pour les tables déchargées
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
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
