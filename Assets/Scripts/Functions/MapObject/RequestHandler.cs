using System;
using System.Collections.Generic;
using System.Text;

namespace MapObject
{
	public class RequestHandler : IActionListener
	{
		// Token: 0x06000A18 RID: 2584 RVA: 0x00008600 File Offset: 0x00006800
		public static RequestHandler getInstance()
		{
			if (RequestHandler._Instance == null)
			{
				RequestHandler._Instance = new RequestHandler();
			}
			return RequestHandler._Instance;
		}

		// Token: 0x06000A19 RID: 2585 RVA: 0x0008AD88 File Offset: 0x00088F88
		public static void update()
		{
			if (global::Char.myCharz().meDead)
			{
				RequestHandler.lastWaitTime = mSystem.currentTimeMillis() + 1000L;
			}
			if (TileMap.mapID == RequestHandler.idMapGoTo)
			{
				RequestHandler.ResetStatus();
				return;
			}
			bool flag = false;
			if (TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23)
			{
				if (RequestHandler.isEatChicken)
				{
					for (int i = 0; i < GameScr.vItemMap.size(); i++)
					{
						ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(i);
						if ((itemMap.playerId == global::Char.myCharz().charID || itemMap.playerId == -1) && itemMap.template.id == 74)
						{
							flag = true;
							global::Char.myCharz().itemFocus = itemMap;
							if (mSystem.currentTimeMillis() - RequestHandler.lastWaitTime > 600L)
							{
								RequestHandler.lastWaitTime = mSystem.currentTimeMillis();
								Service.gI().pickItem(global::Char.myCharz().itemFocus.itemMapID);
								return;
							}
						}
					}
				}
				if (RequestHandler.isXmaping && RequestHandler.isHarvestPean && GameScr.hpPotion < 10 && GameScr.gI().magicTree.currPeas > 0 && mSystem.currentTimeMillis() - RequestHandler.lastWaitTime > 500L)
				{
					RequestHandler.lastWaitTime = mSystem.currentTimeMillis();
					Service.gI().openMenu(4);
					Service.gI().menu(4, 0, 0);
				}
			}
			if (RequestHandler.isXmaping && !flag && mSystem.currentTimeMillis() - RequestHandler.lastWaitTime > 1000L && GameCanvas.gameTick % 50 == 0)
			{
				bool flag2 = true;
				if (RequestHandler.isFutureMap(RequestHandler.idMapGoTo))
				{
					if (flag2 && TileMap.mapID == 27 && GameScr.findNPCInMap(38) == null)
					{
						flag2 = false;
						RequestHandler.StartRunToMapId(28);
					}
					if (flag2 && TileMap.mapID == 29 && GameScr.findNPCInMap(38) == null)
					{
						flag2 = false;
						RequestHandler.StartRunToMapId(28);
					}
					if (flag2 && TileMap.mapID == 28 && GameScr.findNPCInMap(38) == null)
					{
						flag2 = false;
						if (global::Char.myCharz().cx < TileMap.pxw / 2)
						{
							RequestHandler.StartRunToMapId(29);
						}
						else
						{
							RequestHandler.StartRunToMapId(27);
						}
					}
				}
				if (flag2)
				{
					RequestHandler.StartRunToMapId(RequestHandler.idMapGoTo);
				}
			}
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x0008AFB4 File Offset: 0x000891B4
		public void perform(int idAction, object p)
		{
			switch (idAction)
			{
				case 1:
					RequestHandler.ShowPlanetMenu();
					return;
				case 2:
					RequestHandler.isEatChicken = !RequestHandler.isEatChicken;
					GameScr.info1.addInfo("Ăn Đùi Gà\n" + (RequestHandler.isEatChicken ? "[STATUS: ON]" : "[STATUS: OFF]"), 0);
					if (RequestHandler.isSaveData)
					{
						Rms.saveRMSInt("AutoMapIsEatChicken", RequestHandler.isEatChicken ? 1 : 0);
					}
					return;
				case 3:
					RequestHandler.isHarvestPean = !RequestHandler.isHarvestPean;
					GameScr.info1.addInfo("Thu Đậu\n" + (RequestHandler.isHarvestPean ? "[STATUS: ON]" : "[STATUS: OFF]"), 0);
					if (RequestHandler.isSaveData)
					{
						Rms.saveRMSInt("AutoMapIsHarvestPean", RequestHandler.isHarvestPean ? 1 : 0);
					}
					return;
				case 4:
					RequestHandler.isUseCapsule = !RequestHandler.isUseCapsule;
					GameScr.info1.addInfo("Sử Dụng Capsule\n" + (RequestHandler.isUseCapsule ? "[STATUS: ON]" : "[STATUS: OFF]"), 0);
					if (RequestHandler.isSaveData)
					{
						Rms.saveRMSInt("AutoMapIsUseCsb", RequestHandler.isUseCapsule ? 1 : 0);
					}
					return;
				case 5:
					RequestHandler.isSaveData = !RequestHandler.isSaveData;
					GameScr.info1.addInfo("Lưu Cài Đặt Auto Map\n" + (RequestHandler.isSaveData ? "[STATUS: ON]" : "[STATUS: OFF]"), 0);
					Rms.saveRMSInt("AutoMapIsSaveRms", RequestHandler.isSaveData ? 1 : 0);
					if (RequestHandler.isSaveData)
					{
						RequestHandler.SaveData();
					}
					return;
				case 6:
					RequestHandler.ShowMapsMenu((int[])p);
					return;
				case 7:
					RequestHandler.isXmaping = true;
					RequestHandler.idMapGoTo = (int)p;
					GameScr.info1.addInfo("Go to " + TileMap.mapNames[RequestHandler.idMapGoTo], 0);
					return;
				default:
					return;
			}
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x0008B178 File Offset: 0x00089378
		public static void ShowMenu()
		{
			RequestHandler.LoadData();
			MyVector myVector = new MyVector();
			myVector.addElement(new Command("Load Map", RequestHandler.getInstance(), 1, null));
			myVector.addElement(new Command("Ăn Đùi Gà\n" + (RequestHandler.isEatChicken ? "[STATUS: ON]" : "[STATUS: OFF]"), RequestHandler.getInstance(), 2, null));
			myVector.addElement(new Command("Thu Đậu\n" + (RequestHandler.isHarvestPean ? "[STATUS: ON]" : "[STATUS: OFF]"), RequestHandler.getInstance(), 3, null));
			myVector.addElement(new Command("Sử Dụng Capsule\n" + (RequestHandler.isUseCapsule ? "[STATUS: ON]" : "[STATUS: OFF]"), RequestHandler.getInstance(), 4, null));
			myVector.addElement(new Command("Lưu Cài Đặt\n" + (RequestHandler.isSaveData ? "[STATUS: ON]" : "[STATUS: OFF]"), RequestHandler.getInstance(), 5, null));
			GameCanvas.menu.startAt(myVector, 3);
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x0008B270 File Offset: 0x00089470
		private static void ShowPlanetMenu()
		{
			MyVector myVector = new MyVector();
			foreach (KeyValuePair<string, int[]> keyValuePair in RequestHandler.PlanetDictionary)
			{
				myVector.addElement(new Command(keyValuePair.Key, RequestHandler.getInstance(), 6, keyValuePair.Value));
			}
			GameCanvas.menu.startAt(myVector, 3);
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x0008B2F0 File Offset: 0x000894F0
		private static void ShowMapsMenu(int[] a)
		{
			MyVector myVector = new MyVector();
			for (int i = 0; i < a.Length; i++)
			{
				if ((global::Char.myCharz().cgender != 0 || (a[i] != 22 && a[i] != 23)) && (global::Char.myCharz().cgender != 1 || (a[i] != 21 && a[i] != 23)) && (global::Char.myCharz().cgender != 2 || (a[i] != 21 && a[i] != 22)))
				{
					myVector.addElement(new Command(RequestHandler.GetMapName(a[i]), RequestHandler.getInstance(), 7, a[i]));
				}
			}
			GameCanvas.menu.startAt(myVector, 3);
		}

		// Token: 0x06000A1E RID: 2590 RVA: 0x00008618 File Offset: 0x00006818
		public static void Xmap(int a)
		{
			RequestHandler.isXmaping = true;
			RequestHandler.idMapGoTo = a;
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x00008626 File Offset: 0x00006826
		public static void ResetStatus()
		{
			RequestHandler.isXmaping = false;
			RequestHandler.isUsingCapsule = false;
			RequestHandler.isOpeningPanel = false;
		}

		// Token: 0x06000A20 RID: 2592 RVA: 0x0008B390 File Offset: 0x00089590
		public static void StartRunToMapId(int a)
		{
			if (RequestHandler.LinkMaps.ContainsKey(84))
			{
				RequestHandler.LinkMaps.Remove(84);
			}
			RequestHandler.LinkMaps.Add(84, new List<RequestHandler.NextMap>());
			RequestHandler.LinkMaps[84].Add(new RequestHandler.NextMap(24 + global::Char.myCharz().cgender, 10, 0));
			int[] array = RequestHandler.FindWay(a);
			if (array == null)
			{
				GameScr.info1.addInfo("Không thể tìm thấy đường đi", 0);
				return;
			}
			if (RequestHandler.isUseCapsule)
			{
				if (!RequestHandler.isUsingCapsule && array.Length > 3)
				{
					for (int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
					{
						Item item = global::Char.myCharz().arrItemBag[i];
						if (item != null && (item.template.id == 194 || (item.template.id == 193 && item.quantity > 10)))
						{
							RequestHandler.isUsingCapsule = true;
							RequestHandler.isOpeningPanel = false;
							RequestHandler.lastTimeOpenedPanel = mSystem.currentTimeMillis();
							GameCanvas.panel.mapNames = null;
							Service.gI().useItem(0, 1, -1, item.template.id);
							return;
						}
					}
				}
				if (RequestHandler.isUsingCapsule && !RequestHandler.isOpeningPanel && (GameCanvas.panel.mapNames == null || mSystem.currentTimeMillis() - RequestHandler.lastTimeOpenedPanel < 500L))
				{
					return;
				}
				if (RequestHandler.isUsingCapsule && !RequestHandler.isOpeningPanel)
				{
					for (int j = array.Length - 1; j >= 2; j--)
					{
						for (int k = 0; k < GameCanvas.panel.mapNames.Length; k++)
						{
							if (GameCanvas.panel.mapNames[k].Contains(TileMap.mapNames[array[j]]))
							{
								RequestHandler.isOpeningPanel = true;
								Service.gI().requestMapSelect(k);
								return;
							}
						}
					}
					RequestHandler.isOpeningPanel = true;
				}
			}
			if (TileMap.mapID == array[0] && !global::Char.ischangingMap && !Controller.isStopReadMessage)
			{
				RequestHandler.Goto(array[1]);
			}
		}

		// Token: 0x06000A21 RID: 2593 RVA: 0x0000863A File Offset: 0x0000683A
		public static void LoadMapLeft()
		{
			RequestHandler.LoadMap(0);
		}

		// Token: 0x06000A22 RID: 2594 RVA: 0x00008642 File Offset: 0x00006842
		public static void LoadMapCenter()
		{
			RequestHandler.LoadMap(2);
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x0000864A File Offset: 0x0000684A
		public static void LoadMapRight()
		{
			RequestHandler.LoadMap(1);
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x0008B57C File Offset: 0x0008977C
		private static void LoadData()
		{
			RequestHandler.isSaveData = (Rms.loadRMSInt("AutoMapIsSaveRms") == 1);
			if (RequestHandler.isSaveData)
			{
				if (Rms.loadRMSInt("AutoMapIsEatChicken") == -1)
				{
					RequestHandler.isEatChicken = true;
				}
				else
				{
					RequestHandler.isEatChicken = (Rms.loadRMSInt("AutoMapIsEatChicken") == 1);
				}
				if (Rms.loadRMSInt("AutoMapIsUseCsb") == -1)
				{
					RequestHandler.isUseCapsule = true;
				}
				else
				{
					RequestHandler.isUseCapsule = (Rms.loadRMSInt("AutoMapIsUseCsb") == 1);
				}
				RequestHandler.isHarvestPean = (Rms.loadRMSInt("AutoMapIsHarvestPean") == 1);
			}
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x0008B604 File Offset: 0x00089804
		private static void SaveData()
		{
			Rms.saveRMSInt("AutoMapIsEatChicken", RequestHandler.isEatChicken ? 1 : 0);
			Rms.saveRMSInt("AutoMapIsHarvestPean", RequestHandler.isHarvestPean ? 1 : 0);
			Rms.saveRMSInt("AutoMapIsUseCsb", RequestHandler.isUseCapsule ? 1 : 0);
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x0008B650 File Offset: 0x00089850
		private static void LoadLinkMapsXmap()
		{
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			0,
			21
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			1,
			47
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			47,
			111
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			2,
			24
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			5,
			29
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			7,
			22
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			9,
			25
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			13,
			33
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			14,
			23
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			16,
			26
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			20,
			37
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			39,
			21
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			40,
			22
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			41,
			23
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			109,
			105
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			109,
			106
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			106,
			107
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			108,
			105
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			80,
			105
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			3,
			27,
			28,
			29,
			30
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			11,
			31,
			32,
			33,
			34
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			17,
			35,
			36,
			37,
			38
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			109,
			108,
			107,
			110,
			106
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			47,
			46,
			45,
			48
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			131,
			132,
			133
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			42,
			0,
			1,
			2,
			3,
			4,
			5,
			6
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			43,
			7,
			8,
			9,
			11,
			12,
			13,
			10
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			52,
			44,
			14,
			15,
			16,
			17,
			18,
			20,
			19
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			53,
			58,
			59,
			60,
			61,
			62,
			55,
			56,
			54,
			57
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			68,
			69,
			70,
			71,
			72,
			64,
			65,
			63,
			66,
			67,
			73,
			74,
			75,
			76,
			77,
			81,
			82,
			83,
			79,
			80
			});
			RequestHandler.AddLinkMapsXmap(new int[]
			{
			102,
			92,
			93,
			94,
			96,
			97,
			98,
			99,
			100,
			103
			});
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x0008B8F0 File Offset: 0x00089AF0
		private static void LoadNPCLinkMapsXmap()
		{
			RequestHandler.AddNPCLinkMapsXmap(19, 68, 12, 1);
			RequestHandler.AddNPCLinkMapsXmap(19, 109, 12, 0);
			RequestHandler.AddNPCLinkMapsXmap(24, 25, 10, 0);
			RequestHandler.AddNPCLinkMapsXmap(24, 26, 10, 1);
			RequestHandler.AddNPCLinkMapsXmap(24, 84, 10, 2);
			RequestHandler.AddNPCLinkMapsXmap(25, 24, 11, 0);
			RequestHandler.AddNPCLinkMapsXmap(25, 26, 11, 1);
			RequestHandler.AddNPCLinkMapsXmap(25, 84, 11, 2);
			RequestHandler.AddNPCLinkMapsXmap(26, 24, 12, 0);
			RequestHandler.AddNPCLinkMapsXmap(26, 25, 12, 1);
			RequestHandler.AddNPCLinkMapsXmap(26, 84, 12, 2);
			RequestHandler.AddNPCLinkMapsXmap(27, 102, 38, 1);
			RequestHandler.AddNPCLinkMapsXmap(27, 53, 25, 0);
			RequestHandler.AddNPCLinkMapsXmap(28, 102, 38, 1);
			RequestHandler.AddNPCLinkMapsXmap(29, 102, 38, 1);
			RequestHandler.AddNPCLinkMapsXmap(45, 46, 19, 3);
			RequestHandler.AddNPCLinkMapsXmap(52, 127, 44, 0);
			RequestHandler.AddNPCLinkMapsXmap(52, 129, 23, 3);
			RequestHandler.AddNPCLinkMapsXmap(52, 113, 23, 2);
			RequestHandler.AddNPCLinkMapsXmap(68, 19, 12, 0);
			RequestHandler.AddNPCLinkMapsXmap(80, 131, 60, 0);
			RequestHandler.AddNPCLinkMapsXmap(102, 27, 38, 1);
			RequestHandler.AddNPCLinkMapsXmap(113, 52, 22, 4);
			RequestHandler.AddNPCLinkMapsXmap(127, 52, 44, 2);
			RequestHandler.AddNPCLinkMapsXmap(129, 52, 23, 3);
			RequestHandler.AddNPCLinkMapsXmap(131, 80, 60, 1);
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x0008BA44 File Offset: 0x00089C44
		private static void AddPlanetXmap()
		{
			RequestHandler.PlanetDictionary.Add("Trái đất", RequestHandler.idMapTraiDat);
			RequestHandler.PlanetDictionary.Add("Namếc", RequestHandler.idMapNamek);
			RequestHandler.PlanetDictionary.Add("Xayda", RequestHandler.idMapXayda);
			RequestHandler.PlanetDictionary.Add("Fide", RequestHandler.idMapNappa);
			RequestHandler.PlanetDictionary.Add("Tương lai", RequestHandler.idMapTuongLai);
			RequestHandler.PlanetDictionary.Add("Cold", RequestHandler.idMapCold);
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x0008BACC File Offset: 0x00089CCC
		private static void AddLinkMapsXmap(params int[] a)
		{
			for (int i = 0; i < a.Length; i++)
			{
				if (!RequestHandler.LinkMaps.ContainsKey(a[i]))
				{
					RequestHandler.LinkMaps.Add(a[i], new List<RequestHandler.NextMap>());
				}
				if (i != 0)
				{
					RequestHandler.LinkMaps[a[i]].Add(new RequestHandler.NextMap(a[i - 1], -1, -1));
				}
				if (i != a.Length - 1)
				{
					RequestHandler.LinkMaps[a[i]].Add(new RequestHandler.NextMap(a[i + 1], -1, -1));
				}
			}
		}

		// Token: 0x06000A2A RID: 2602 RVA: 0x00008652 File Offset: 0x00006852
		private static void AddNPCLinkMapsXmap(int a, int b, int c, int d)
		{
			if (!RequestHandler.LinkMaps.ContainsKey(a))
			{
				RequestHandler.LinkMaps.Add(a, new List<RequestHandler.NextMap>());
			}
			RequestHandler.LinkMaps[a].Add(new RequestHandler.NextMap(b, c, d));
		}

		// Token: 0x06000A2B RID: 2603 RVA: 0x0008BB50 File Offset: 0x00089D50
		private static void Goto(int a)
		{
			foreach (RequestHandler.NextMap nextMap in RequestHandler.LinkMaps[TileMap.mapID])
			{
				if (nextMap.MapID == a)
				{
					nextMap.GotoMap();
					return;
				}
			}
			GameScr.info1.addInfo("Không thể thực hiện", 0);
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x00008689 File Offset: 0x00006889
		private static int[] FindWay(int a)
		{
			return RequestHandler.FindWay(a, new int[]
			{
			TileMap.mapID
			});
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x0008BBCC File Offset: 0x00089DCC
		private static int[] FindWay(int a, int[] b)
		{
			List<int[]> list = new List<int[]>();
			List<int> list2 = new List<int>();
			list2.AddRange(b);
			foreach (RequestHandler.NextMap nextMap in RequestHandler.LinkMaps[b[b.Length - 1]])
			{
				if (a == nextMap.MapID)
				{
					list2.Add(a);
					return list2.ToArray();
				}
				if (!list2.Contains(nextMap.MapID))
				{
					List<int> list3 = new List<int>(list2)
				{
					nextMap.MapID
				};
					int[] array = RequestHandler.FindWay(a, list3.ToArray());
					if (array != null)
					{
						list.Add(array);
					}
				}
			}
			int num = 9999;
			int[] result = null;
			foreach (int[] array2 in list)
			{
				if (!RequestHandler.hasWayGoFutureAndBack(array2) && (global::Char.myCharz().taskMaint.taskId > 30 || !RequestHandler.hasWayGoToColdMap(array2)) && array2.Length < num)
				{
					num = array2.Length;
					result = array2;
				}
			}
			return result;
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x0008BD10 File Offset: 0x00089F10
		private static bool hasWayGoFutureAndBack(int[] a)
		{
			for (int i = 1; i < a.Length - 1; i++)
			{
				if (a[i] == 102 && a[i + 1] == 24)
				{
					if (a[i - 1] != 27 && a[i - 1] != 28)
					{
						if (a[i - 1] != 29)
						{
							goto IL_31;
						}
					}
					return true;
				}
				IL_31:;
			}
			return false;
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x0008BD60 File Offset: 0x00089F60
		private static bool hasWayGoToColdMap(int[] a)
		{
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] >= 105 && a[i] <= 110)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000A30 RID: 2608 RVA: 0x0008BD8C File Offset: 0x00089F8C
		private static string GetMapName(int a)
		{
			string result;
			if (a != 113)
			{
				if (a == 129)
				{
					result = TileMap.mapNames[a] + " 23\n[" + a.ToString() + "]";
				}
				else
				{
					result = TileMap.mapNames[a] + "\n[" + a.ToString() + "]";
				}
			}
			else
			{
				result = string.Concat(new object[]
				{
				"Siêu hạng\n[",
				a,
				"]"
				});
			}
			return result;
		}

		// Token: 0x06000A31 RID: 2609 RVA: 0x0008BE0C File Offset: 0x0008A00C
		private static void LoadWaypointsInMap()
		{
			RequestHandler.ResetSavedWaypoints();
			int num = TileMap.vGo.size();
			if (num != 2)
			{
				for (int i = 0; i < num; i++)
				{
					Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
					if (waypoint.maxX < 60)
					{
						RequestHandler.wayPointMapLeft[0] = (int)(waypoint.minX + 15);
						RequestHandler.wayPointMapLeft[1] = (int)waypoint.maxY;
					}
					else if ((int)waypoint.maxX > TileMap.pxw - 60)
					{
						RequestHandler.wayPointMapRight[0] = (int)(waypoint.maxX - 15);
						RequestHandler.wayPointMapRight[1] = (int)waypoint.maxY;
					}
					else
					{
						RequestHandler.wayPointMapCenter[0] = (int)(waypoint.minX + 15);
						RequestHandler.wayPointMapCenter[1] = (int)waypoint.maxY;
					}
				}
				return;
			}
			Waypoint waypoint2 = (Waypoint)TileMap.vGo.elementAt(0);
			Waypoint waypoint3 = (Waypoint)TileMap.vGo.elementAt(1);
			if ((waypoint2.maxX < 60 && waypoint3.maxX < 60) || ((int)waypoint2.minX > TileMap.pxw - 60 && (int)waypoint3.minX > TileMap.pxw - 60))
			{
				RequestHandler.wayPointMapLeft[0] = (int)(waypoint2.minX + 15);
				RequestHandler.wayPointMapLeft[1] = (int)waypoint2.maxY;
				RequestHandler.wayPointMapRight[0] = (int)(waypoint3.maxX - 15);
				RequestHandler.wayPointMapRight[1] = (int)waypoint3.maxY;
				return;
			}
			if (waypoint2.maxX < waypoint3.maxX)
			{
				RequestHandler.wayPointMapLeft[0] = (int)(waypoint2.minX + 15);
				RequestHandler.wayPointMapLeft[1] = (int)waypoint2.maxY;
				RequestHandler.wayPointMapRight[0] = (int)(waypoint3.maxX - 15);
				RequestHandler.wayPointMapRight[1] = (int)waypoint3.maxY;
				return;
			}
			RequestHandler.wayPointMapLeft[0] = (int)(waypoint3.minX + 15);
			RequestHandler.wayPointMapLeft[1] = (int)waypoint3.maxY;
			RequestHandler.wayPointMapRight[0] = (int)(waypoint2.maxX - 15);
			RequestHandler.wayPointMapRight[1] = (int)waypoint2.maxY;
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x0008BFE8 File Offset: 0x0008A1E8
		private static int GetYGround(int a)
		{
			int num = 50;
			int i = 0;
			while (i < 30)
			{
				i++;
				num += 24;
				if (TileMap.tileTypeAt(a, num, 2))
				{
					if (num % 24 != 0)
					{
						num -= num % 24;
					}
					return num;
				}
			}
			return num;
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x0008C024 File Offset: 0x0008A224
		private static void TeleportTo(int a, int b)
		{
			if (GameScr.canAutoPlay)
			{
				global::Char.myCharz().cx = a;
				global::Char.myCharz().cy = b;
				Service.gI().charMove();
				return;
			}
			global::Char.myCharz().cx = a;
			global::Char.myCharz().cy = b;
			Service.gI().charMove();
			global::Char.myCharz().cx = a;
			global::Char.myCharz().cy = b + 1;
			Service.gI().charMove();
			global::Char.myCharz().cx = a;
			global::Char.myCharz().cy = b;
			Service.gI().charMove();
		}

		// Token: 0x06000A34 RID: 2612 RVA: 0x0000869F File Offset: 0x0000689F
		private static void ResetSavedWaypoints()
		{
			RequestHandler.wayPointMapLeft = new int[2];
			RequestHandler.wayPointMapCenter = new int[2];
			RequestHandler.wayPointMapRight = new int[2];
		}

		// Token: 0x06000A35 RID: 2613 RVA: 0x000086C2 File Offset: 0x000068C2
		private static bool isNRDMap(int a)
		{
			return a >= 85 && a <= 91;
		}

		// Token: 0x06000A36 RID: 2614 RVA: 0x0008C0BC File Offset: 0x0008A2BC
		private static bool isFutureMap(int a)
		{
			for (int i = 0; i < RequestHandler.idMapTuongLai.Length; i++)
			{
				if (RequestHandler.idMapTuongLai[i] == a)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000A37 RID: 2615 RVA: 0x000086D3 File Offset: 0x000068D3
		private static bool isNRD(ItemMap a)
		{
			return a.template.id >= 372 && a.template.id <= 378;
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x0008C0EC File Offset: 0x0008A2EC
		private static void LoadMap(int a)
		{
			if (RequestHandler.isNRDMap(TileMap.mapID))
			{
				RequestHandler.isClanXmap(a);
				return;
			}
			RequestHandler.LoadWaypointsInMap();
			switch (a)
			{
				case 0:
					if (RequestHandler.wayPointMapLeft[0] != 0 && RequestHandler.wayPointMapLeft[1] != 0)
					{
						RequestHandler.TeleportTo(RequestHandler.wayPointMapLeft[0], RequestHandler.wayPointMapLeft[1]);
					}
					else
					{
						RequestHandler.TeleportTo(60, RequestHandler.GetYGround(60));
					}
					break;
				case 1:
					if (RequestHandler.wayPointMapRight[0] != 0 && RequestHandler.wayPointMapRight[1] != 0)
					{
						RequestHandler.TeleportTo(RequestHandler.wayPointMapRight[0], RequestHandler.wayPointMapRight[1]);
					}
					else
					{
						RequestHandler.TeleportTo(TileMap.pxw - 60, RequestHandler.GetYGround(TileMap.pxw - 60));
					}
					break;
				case 2:
					if (RequestHandler.wayPointMapCenter[0] != 0 && RequestHandler.wayPointMapCenter[1] != 0)
					{
						RequestHandler.TeleportTo(RequestHandler.wayPointMapCenter[0], RequestHandler.wayPointMapCenter[1]);
					}
					else
					{
						RequestHandler.TeleportTo(TileMap.pxw / 2, RequestHandler.GetYGround(TileMap.pxw / 2));
					}
					break;
			}
			if (TileMap.mapID != 7 && TileMap.mapID != 14 && TileMap.mapID != 0)
			{
				Service.gI().requestChangeMap();
				return;
			}
			Service.gI().getMapOffline();
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x0008C214 File Offset: 0x0008A414
		private static void isClanXmap(int a)
		{
			if (a == 0)
			{
				RequestHandler.TeleportTo(60, RequestHandler.GetYGround(60));
				return;
			}
			if (a == 2)
			{
				if (global::Char.myCharz().bag >= 0 && ClanImage.idImages.containsKey(global::Char.myCharz().bag.ToString()))
				{
					ClanImage clanImage = (ClanImage)ClanImage.idImages.get(global::Char.myCharz().bag.ToString());
					if (clanImage.idImage != null)
					{
						for (int i = 0; i < clanImage.idImage.Length; i++)
						{
							if (clanImage.idImage[i] == 2322)
							{
								for (int j = 0; j < GameScr.vNpc.size(); j++)
								{
									Npc npc = (Npc)GameScr.vNpc.elementAt(j);
									if (npc.template.npcTemplateId >= 30 && npc.template.npcTemplateId <= 36)
									{
										global::Char.myCharz().npcFocus = npc;
										RequestHandler.TeleportTo(npc.cx, npc.cy - 3);
										return;
									}
								}
							}
						}
					}
				}
				for (int k = 0; k < GameScr.vItemMap.size(); k++)
				{
					ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(k);
					if (itemMap != null && RequestHandler.isNRD(itemMap))
					{
						global::Char.myCharz().itemFocus = itemMap;
						RequestHandler.TeleportTo(itemMap.x, itemMap.y);
						return;
					}
				}
				return;
			}
			RequestHandler.TeleportTo(TileMap.pxw - 60, RequestHandler.GetYGround(TileMap.pxw - 60));
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x0008C390 File Offset: 0x0008A590
		static RequestHandler()
		{
			RequestHandler.LoadLinkMapsXmap();
			RequestHandler.LoadNPCLinkMapsXmap();
			RequestHandler.AddPlanetXmap();
			RequestHandler.LoadData();
		}

		// Token: 0x04001359 RID: 4953
		public static RequestHandler _Instance;

		// Token: 0x0400135A RID: 4954
		private static Dictionary<int, List<RequestHandler.NextMap>> LinkMaps = new Dictionary<int, List<RequestHandler.NextMap>>();

		// Token: 0x0400135B RID: 4955
		private static Dictionary<string, int[]> PlanetDictionary = new Dictionary<string, int[]>();

		// Token: 0x0400135C RID: 4956
		public static bool isXmaping;

		// Token: 0x0400135D RID: 4957
		public static int idMapGoTo;

		// Token: 0x0400135E RID: 4958
		private static int[] wayPointMapLeft;

		// Token: 0x0400135F RID: 4959
		private static int[] wayPointMapCenter;

		// Token: 0x04001360 RID: 4960
		private static int[] wayPointMapRight;

		// Token: 0x04001361 RID: 4961
		private static bool isEatChicken = true;

		// Token: 0x04001362 RID: 4962
		private static bool isHarvestPean;

		// Token: 0x04001363 RID: 4963
		private static bool isUseCapsule = true;

		// Token: 0x04001364 RID: 4964
		private static bool isUsingCapsule;

		// Token: 0x04001365 RID: 4965
		private static bool isOpeningPanel;

		// Token: 0x04001366 RID: 4966
		private static long lastTimeOpenedPanel;

		// Token: 0x04001367 RID: 4967
		private static bool isSaveData;

		// Token: 0x04001368 RID: 4968
		private static long lastWaitTime;

		// Token: 0x04001369 RID: 4969
		private static int[] idMapNamek = new int[]
		{
		43,
		22,
		7,
		8,
		9,
		11,
		12,
		13,
		10,
		31,
		32,
		33,
		34,
		43,
		25
		};

		// Token: 0x0400136A RID: 4970
		private static int[] idMapXayda = new int[]
		{
		44,
		23,
		14,
		15,
		16,
		17,
		18,
		20,
		19,
		35,
		36,
		37,
		38,
		52,
		44,
		26,
		84,
		113,
		127,
		129
		};

		// Token: 0x0400136B RID: 4971
		private static int[] idMapTraiDat = new int[]
		{
		42,
		21,
		0,
		1,
		2,
		3,
		4,
		5,
		6,
		27,
		28,
		29,
		30,
		47,
		42,
		24,
		46,
		45,
		48,
		53,
		58,
		59,
		60,
		61,
		62,
		55,
		56,
		54,
		57
		};

		// Token: 0x0400136C RID: 4972
		private static int[] idMapTuongLai = new int[]
		{
		102,
		92,
		93,
		94,
		96,
		97,
		98,
		99,
		100,
		103
		};

		// Token: 0x0400136D RID: 4973
		private static int[] idMapCold = new int[]
		{
		109,
		108,
		107,
		110,
		106,
		105
		};

		// Token: 0x0400136E RID: 4974
		private static int[] idMapNappa = new int[]
		{
		68,
		69,
		70,
		71,
		72,
		64,
		65,
		63,
		66,
		67,
		73,
		74,
		75,
		76,
		77,
		81,
		82,
		83,
		79,
		80,
		131,
		132,
		133
		};

		// Token: 0x020000ED RID: 237
		public class NextMap
		{
			// Token: 0x06000A3C RID: 2620 RVA: 0x000086FE File Offset: 0x000068FE
			public NextMap(int a, int b, int c)
			{
				this.MapID = a;
				this.Npc = b;
				this.Index = c;
			}

			// Token: 0x06000A3D RID: 2621 RVA: 0x0008C45C File Offset: 0x0008A65C
			public void GotoMap()
			{
				if (this.Index == -1 && this.Npc == -1)
				{
					Waypoint wayPoint = this.GetWayPoint();
					if (wayPoint != null)
					{
						this.Enter(wayPoint);
						return;
					}
				}
				else if (this.Npc != -1 && this.Index != -1)
				{
					Service.gI().openMenu(this.Npc);
					Service.gI().confirmMenu(0, (sbyte)this.Index);
				}
			}

			// Token: 0x06000A3E RID: 2622 RVA: 0x0008C4C4 File Offset: 0x0008A6C4
			public Waypoint GetWayPoint()
			{
				for (int i = 0; i < TileMap.vGo.size(); i++)
				{
					Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
					if (this.GetMapName().Equals(this.GetMapName(waypoint.popup)))
					{
						return waypoint;
					}
				}
				return null;
			}

			// Token: 0x06000A3F RID: 2623 RVA: 0x0000871B File Offset: 0x0000691B
			public string GetMapName()
			{
				return TileMap.mapNames[this.MapID];
			}

			// Token: 0x06000A40 RID: 2624 RVA: 0x0008C514 File Offset: 0x0008A714
			public void Enter(Waypoint a)
			{
				int num = (a.maxX < 60) ? 15 : (((int)a.minX <= TileMap.pxw - 60) ? ((int)((a.minX + a.maxX) / 2)) : (TileMap.pxw - 15));
				int maxY = (int)a.maxY;
				if (num != -1)
				{
					if (maxY != -1)
					{
						this.TeleportTo(num, maxY);
						if (a.isOffline)
						{
							Service.gI().getMapOffline();
							return;
						}
						Service.gI().requestChangeMap();
						return;
					}
				}
				GameScr.info1.addInfo("Có lỗi xảy ra", 0);
			}

			// Token: 0x06000A41 RID: 2625 RVA: 0x0008C5A0 File Offset: 0x0008A7A0
			public string GetMapName(PopUp a)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < a.says.Length; i++)
				{
					stringBuilder.Append(a.says[i]);
					stringBuilder.Append(" ");
				}
				return stringBuilder.ToString().Trim();
			}

			// Token: 0x06000A42 RID: 2626 RVA: 0x0008C5EC File Offset: 0x0008A7EC
			public void TeleportTo(int a, int b)
			{
				if (GameScr.canAutoPlay)
				{
					global::Char.myCharz().cx = a;
					global::Char.myCharz().cy = b;
					Service.gI().charMove();
					return;
				}
				global::Char.myCharz().cx = a;
				global::Char.myCharz().cy = b;
				Service.gI().charMove();
				global::Char.myCharz().cx = a;
				global::Char.myCharz().cy = b + 1;
				Service.gI().charMove();
				global::Char.myCharz().cx = a;
				global::Char.myCharz().cy = b;
				Service.gI().charMove();
			}

			// Token: 0x0400136F RID: 4975
			public int MapID;

			// Token: 0x04001370 RID: 4976
			public int Npc;

			// Token: 0x04001371 RID: 4977
			public int Index;
		}
	}
}