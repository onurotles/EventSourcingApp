namespace EventSourcingApp {
    class Program {
        static void Main() {
            var warehouseProductRepository = new WarehouseProductRepository();

            var key = string.Empty;
            while (key != "X") {
                Console.WriteLine("R: Receive Inventory");
                Console.WriteLine("S: Ship Inventory");
                Console.WriteLine("A: Inventory Adjustment");
                Console.WriteLine("Q: Quantity On Hand");
                Console.WriteLine("E: Events");
                Console.Write("> ");
                key = Console.ReadLine()?.ToUpperInvariant();
                Console.WriteLine();

                var sku = GetSkuFromConsole();
                var warehouseProduct = warehouseProductRepository.Get(sku);

                switch (key) {
                    case "R":
                        var receiveInput = GetQuantity();
                        if (receiveInput.IsValid) {
                            warehouseProduct.ReceiveProduct(receiveInput.Quantity);
                            Console.WriteLine($"{sku} Received: {receiveInput.Quantity}");
                        }
                        break;
                    case "S":
                        var shipInput = GetQuantity();
                        if (shipInput.IsValid) {
                            warehouseProduct.ShipProduct(shipInput.Quantity);
                            Console.WriteLine($"{sku} Shipped: {shipInput.Quantity}");
                        }
                        break;
                    case "A":
                        var adjustmentInput = GetQuantity();
                        if (adjustmentInput.IsValid) {
                            var reason = GetAdjustmentReason();
                            warehouseProduct.AdjustInventory(adjustmentInput.Quantity, reason);
                            Console.WriteLine($"{sku} Adjusted: {adjustmentInput.Quantity}");
                        }
                        break;
                    case "Q":
                        var currentQuantityOnHand = warehouseProduct.GetQuantityOnHand();
                        Console.WriteLine($"{sku} Quantity On Hand: {currentQuantityOnHand}");
                        break;
                    case "E":
                        Console.WriteLine($"Events: {sku}");
                        foreach (var evnt in warehouseProduct.GetEvents()) {
                            switch (evnt) {
                                case ProductShipped shipProduct:
                                    Console.WriteLine($"{shipProduct.DateTime:u} {sku} Shipped: {shipProduct.Quantity}");
                                    break;
                                case ProductReceived receiveProduct:
                                    Console.WriteLine($"{receiveProduct.DateTime:u} {sku} Received: {receiveProduct.Quantity}");
                                    break;
                                case InventoryAdjusted inventoryAdjusted:
                                    Console.WriteLine($"{inventoryAdjusted.DateTime:u} {sku} Adjusted: {inventoryAdjusted.Quantity}");
                                    break;
                            }
                        }

                        break;
                }

                warehouseProductRepository.Save(warehouseProduct);

                Console.ReadLine();
                Console.WriteLine();
            }
        }

        private static string GetSkuFromConsole() {
            Console.Write("SKU: ");
            var sku = Console.ReadLine();
            return sku;
        }

        private static string GetAdjustmentReason() {
            Console.Write("Reason: ");
            var reason = Console.ReadLine();
            return reason;
        }

        public class QuantityInfo {
            public int Quantity { get; set; }
            public bool IsValid { get; set; }
        }

        private static QuantityInfo GetQuantity() {
            Console.Write("Quantity: ");
            var quantity = Console.ReadLine();
            int quantityInt;
            var quantityEntry = new QuantityInfo { IsValid = Int32.TryParse(quantity, out quantityInt), Quantity = quantityInt };
            return quantityEntry;
        }
    }
}