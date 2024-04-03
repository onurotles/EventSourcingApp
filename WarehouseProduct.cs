namespace EventSourcingApp {
    public class WarehouseProduct {
        public string Sku { get; }
        private readonly IList<IEvent> _events = new List<IEvent>();

        private readonly CurrentState _currentState = new();

        public WarehouseProduct(string sku){
            Sku = sku;
        }

        public void ShipProduct(int quantity) {
            if (quantity > _currentState.Quantity) {
                throw new InvalidDomainException("We don't have enough product to ship");
            }

            AddEvent(new ProductShipped(Sku, quantity, DateTime.UtcNow));
        }

        public void ReceiveProduct(int quantity) {
            AddEvent(new ProductReceived(Sku, quantity, DateTime.UtcNow));
        }

        public void AdjustInventory(int quantity, string reason) {
            if (_currentState.Quantity + quantity < 0) {
                throw new InvalidDomainException("Cannot adjust");
            }

            AddEvent(new InventoryAdjusted(Sku, quantity, reason, DateTime.UtcNow));
        }

        public IList<IEvent> GetEvents() {
            return _events;
        }

        public void AddEvent(IEvent evnt) {
            switch (evnt) {
                case ProductShipped shipProduct:
                    Apply(shipProduct);
                    break;
                case ProductReceived receiveProduct:
                    Apply(receiveProduct);
                    break;
                case InventoryAdjusted inventoryAdjusted:
                    Apply(inventoryAdjusted);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported");
            }

            _events.Add(evnt);
        }

        private void Apply(ProductShipped evnt) {
            _currentState.Quantity -= evnt.Quantity;
        }

        private void Apply(ProductReceived evnt) {
            _currentState.Quantity += evnt.Quantity;
        }

        private void Apply(InventoryAdjusted evnt) {
            _currentState.Quantity += evnt.Quantity;
        }

        public int GetQuantityOnHand() {
            return _currentState.Quantity;
        }
    }

    public class InvalidDomainException : Exception {
        public InvalidDomainException(string message) : base(message) {

        }
    }
}