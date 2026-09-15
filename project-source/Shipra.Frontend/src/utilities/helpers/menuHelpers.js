import React from "react";
import { getMenuIcon } from "../../components/sideNavBar/icons";
import { handleGetDraftOrdersCount } from "../../components/authRoutes";

/**
 * Transforms GetAllClientMenuByRoleId API response into navBarMenu format for PrivateRoutes and SideNavBar.
 * Merges API menuOtherRoutes with static navBarMenu's otherRoutes to maintain full route coverage.
 */
export const transformApiMenuToNavBarMenu = (
  apiMenuItems = [],
  LanguageReducer,
  staticNavBarMenu = []
) => {
  if (!Array.isArray(apiMenuItems) || apiMenuItems.length === 0) {
    return [];
  }

  // 1. Only include main menus where hasPermission !== false
  const allowedMainMenus = apiMenuItems.filter(
    (menu) => menu.hasPermission === true || menu.hasPermission !== false
  );

  // 2. Sort top-level menus by displayOrder
  const sortedMenus = [...allowedMainMenus].sort(
    (a, b) => (a.displayOrder || 0) - (b.displayOrder || 0)
  );

  return sortedMenus.map((menu) => {
    const rawItems = menu.menuItems || [];

    // 3. Only include sub-items where hasPermission !== false
    const validSubItems = rawItems.filter(
      (sub) => sub.hasPermission === true || sub.hasPermission !== false
    );

    const sortedSubItems = [...validSubItems].sort(
      (a, b) => (a.displayOrder || 0) - (b.displayOrder || 0)
    );

    // Set isCollapse = true whenever allowed sub-items exist
    const isCollapse = sortedSubItems.length > 0;

    const subTabData = sortedSubItems.map((sub) => ({
      menuItemId: sub.menuItemId,
      sideTitle: sub.menuItemName || sub.menuItemTabBarTitle || sub.tabBarTitle,
      topTitle: sub.menuItemTabBarTitle || sub.menuItemName || sub.tabBarTitle,
      path: sub.routePath,
      displayOrder: sub.displayOrder,
      hasPermission: sub.hasPermission,
      hasBadge: sub.routePath === "/draft-order-dashboard",
      handleShowBadge:
        sub.routePath === "/draft-order-dashboard"
          ? handleGetDraftOrdersCount
          : null,
    }));

    // Find matching static menu to fetch default subTabData and otherRoutes
    const matchedStaticMenu = (staticNavBarMenu || []).find(
      (st) => st?.tabData?.path === menu.routePath
    );

    // Merge static subTabData if not present in API items (e.g. newly created routes like /sync-policies)
    if (matchedStaticMenu?.subTabData) {
      matchedStaticMenu.subTabData.forEach((stSub) => {
        if (
          stSub?.path &&
          !subTabData.some((s) => s.path === stSub.path)
        ) {
          subTabData.push(stSub);
        }
      });
    }

    // Map API menuOtherRoutes
    const apiOtherRoutes = (menu.menuOtherRoutes || []).map((other) => ({
      topTitle: other.tabBarTitle || other.topTitle || other.menuItemName,
      path: other.routePath || other.path,
    }));

    // Merge API otherRoutes with static otherRoutes (avoiding duplicates)
    const combinedOtherRoutes = [...apiOtherRoutes];

    if (matchedStaticMenu?.otherRoutes) {
      matchedStaticMenu.otherRoutes.forEach((stOther) => {
        if (
          stOther?.path &&
          !combinedOtherRoutes.some((o) => o.path === stOther.path)
        ) {
          combinedOtherRoutes.push(stOther);
        }
      });
    }

    return {
      menuId: menu.menuId,
      displayOrder: menu.displayOrder,
      hasPermission: menu.hasPermission,
      isCollapse: isCollapse,
      tabData: {
        sideTitle: menu.menuName || menu.menuTabBarTitle || menu.tabBarTitle,
        topTitle: menu.menuTabBarTitle || menu.menuName || menu.tabBarTitle,
        path: menu.routePath,
      },
      subTabData: subTabData,
      otherRoutes: combinedOtherRoutes,
      icon: getMenuIcon(menu.menuIcon || menu.menuName),
    };
  });
};
